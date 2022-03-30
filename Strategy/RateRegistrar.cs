using Aweton.Labs.CurrencyRates.Cli.Data;
using Aweton.Labs.CurrencyRates.Cli.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aweton.Labs.CurrencyRates.Cli.Strategy;
internal class RateRegistrar
{
  private readonly IMiceDbContext m_DbContext;
  private readonly int m_RateTypeId;
  private readonly IEqualityComparer<CurrencyRate> m_Comparer;

  public RateRegistrar(IMiceDbContext dbContext, IOptions<RateRegistrarOptions> options, IEqualityComparer<CurrencyRate> comparer)
  {
    m_DbContext = dbContext;
    m_RateTypeId = options.Value.RateTypeId;
    m_Comparer = comparer;
  }

  public async Task Register(IReadOnlyList<CurrencyRate> fetched)
  {
    if (fetched.Count <= 0)
    {
      return;
    }

    fetched.Aggregate(
      m_DbContext.AddCurrencyRate, 
      await CreateReducer(
        fetched.Min((e) => e.ADate), 
        fetched.Max((e) => e.ADate)
      )
    );

    await m_DbContext.SaveChangesAsync(CancellationToken.None);
  }

  private async Task<Func<Action<CurrencyRate>, CurrencyRate, Action<CurrencyRate>>> CreateReducer(DateTime min, DateTime max)
  {
    Func<CurrencyRate, CurrencyRate?> finder = CreateFinder(
      await m_DbContext.GetCurrentState(min,max)
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
    found.UserName = $"registrar ${DateTime.Now.ToString("O")}";
  }

  private CurrencyRate CreateRecordToInsert(CurrencyRate e)
  {
    return new CurrencyRate
    {
      ADate = e.ADate,
      CurrencyTypesID = e.CurrencyTypesID,
      Rate = e.Rate,
      RateTypesID = m_RateTypeId
    };
  }

}