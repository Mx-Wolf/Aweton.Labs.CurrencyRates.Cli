
using Microsoft.EntityFrameworkCore;

namespace Aweton.Labs.CurrencyRates.Cli.Data;

internal class CurrencyCodesWorker : ICurrencyCodesWorker
{
  private IReadOnlyDictionary<string, int>? m_Cache = null;
  private readonly MiceDbContext m_DbContext;

  public CurrencyCodesWorker(MiceDbContext dbContext)
  {
    m_DbContext = dbContext;
  }

  public async Task<IReadOnlyDictionary<string, int>> Load()
  {
    if (m_Cache == null)
    {
      m_Cache = await Create();
    }
    return m_Cache;
  }

  private async Task<IReadOnlyDictionary<string, int>> Create()
  {
    var list = await m_DbContext.CurrencyTypes.Select(e => new { e.CurrencyTypesID, e.CurrCodeChr }).ToListAsync();
    return list.ToDictionary((e) => e.CurrCodeChr, (e) => e.CurrencyTypesID);
  }

}
