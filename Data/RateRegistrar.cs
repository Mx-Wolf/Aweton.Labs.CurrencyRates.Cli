using Aweton.Labs.CurrencyRates.Cli.Models;

namespace Aweton.Labs.CurrencyRates.Cli.Data;
internal class RegistrarWorker : IRegistarWorker
{
  private readonly IRegistarPersister m_DbContext;
  private readonly IEqualityComparer<CurrencyRate> m_Comparer;

  public RegistrarWorker(IRegistarPersister dbContext, IEqualityComparer<CurrencyRate> comparer)
  {
    m_DbContext = dbContext;
    m_Comparer = comparer;
  }

  public async Task<int> Register(IReadOnlyList<CurrencyRate> fetched)
  {
    if (fetched.Count <= 0)
    {
      return 0;
    }

    fetched.Aggregate(
      m_DbContext.AddCurrencyRate,
      await CreateReducer(
        fetched.Min((e) => e.ADate),
        fetched.Max((e) => e.ADate)
      )
    );

    return await m_DbContext.SaveChangesAsync(CancellationToken.None);
  }

  private async Task<Func<Action<CurrencyRate>, CurrencyRate, Action<CurrencyRate>>> CreateReducer(DateTime min, DateTime max)
  {
    Func<CurrencyRate, CurrencyRate?> finder = CreateFinder(
      await m_DbContext.GetCurrentState(min, max)
    );
    return (Action<CurrencyRate> addDelegate, CurrencyRate e) =>
    {
      InsertOrUpdateRecord(addDelegate, e, finder(e));
      return addDelegate;
    };
  }

  private Func<CurrencyRate, CurrencyRate?> CreateFinder(IReadOnlyList<CurrencyRate> registeredRates)
  {
    return (CurrencyRate item) => registeredRates.FirstOrDefault((registered) => m_Comparer.Equals(item, registered));
  }

  private void InsertOrUpdateRecord(Action<CurrencyRate> addDelegate, CurrencyRate e, CurrencyRate? found)
  {
    if (found != null)
    {
      UpdateExistingRecord(e, found);
    }
    else
    {
      addDelegate(CreateRecordToInsert(e));
    }
  }

  private static void UpdateExistingRecord(CurrencyRate e, CurrencyRate found)
  {
    found.Rate = e.Rate;
    found.UserName = $"updated ${DateTime.Now.ToString("O")}";
  }

  private static CurrencyRate CreateRecordToInsert(CurrencyRate e) => e;

}