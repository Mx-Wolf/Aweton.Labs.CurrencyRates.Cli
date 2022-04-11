using Aweton.Labs.CurrencyRates.Cli.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aweton.Labs.CurrencyRates.Cli.Data;
internal class RegistrarWorker : IRegistarWorker
{
  private readonly MiceDbContext m_DbContext;
  private readonly IEqualityComparer<CurrencyRate> m_Comparer;
  private readonly IMiceClock m_MiceClock;
  private readonly int m_RateTypesId;

  public RegistrarWorker(MiceDbContext dbContext, IEqualityComparer<CurrencyRate> comparer, IMiceClock miceClock, IOptions<MiceDbSettings> settings)
  {
    m_DbContext = dbContext;
    m_Comparer = comparer;
    m_MiceClock = miceClock;
    m_RateTypesId = settings.Value.RateTypesId;
  }

  public async Task<int> Register(IReadOnlyList<CurrencyRate> fetched)
  {
    if (fetched.Count <= 0)
    {
      return 0;
    }
    var max = fetched.Max((e) => e.ADate);
    fetched.Aggregate(
      (a) => m_DbContext.CurrencyRates.Add(a),
      await CreateReducer(
        fetched.Min((e) => e.ADate),
        max
      )
    );

    m_DbContext.awjCbrDailyLogs.Add(new awjCbrDailyLog
    {
      aDate = max,
      awjCbrDailyLogID = 0,
      HashBytes = new byte[0],
      PostedAt = m_MiceClock.GetUtcDate(),
      Rows = fetched.Count
    });
    int result = await m_DbContext.SaveChangesAsync(CancellationToken.None);
    await UpdateActualRateValues();
    return result;
    
  }

  private async Task UpdateActualRateValues()
  {
    await m_DbContext.Database.ExecuteSqlRawAsync("sys_UpdateCurrencyRatesNow");
  }

  private async Task<Func<Action<CurrencyRate>, CurrencyRate, Action<CurrencyRate>>> CreateReducer(DateTime first, DateTime last)
  {
    Func<CurrencyRate, CurrencyRate?> finder = CreateFinder(
      await m_DbContext.CurrencyRates.Where(
      (e) => (
        e.ADate >= first
        && e.ADate <= last
        && e.RateTypesID == m_RateTypesId
        )
      ).ToListAsync()
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

  private void UpdateExistingRecord(CurrencyRate e, CurrencyRate found)
  {
    if (e.Rate != found.Rate)
    {
      found.Rate = e.Rate;
      found.UserName = $"updated {m_MiceClock.GetUtcDate().ToString("O")}";
    }
  }

  private static CurrencyRate CreateRecordToInsert(CurrencyRate e) => e;

}