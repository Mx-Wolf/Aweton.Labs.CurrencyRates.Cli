using Aweton.Labs.CurrencyRates.Cli.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aweton.Labs.CurrencyRates.Cli.Data;

public class RegistarPersister : IRegistarPersister
{
  private readonly int m_RateTypeId;
  private readonly MiceDbContext m_DbContext;
  public RegistarPersister(IOptions<MiceDbSettings> settings, MiceDbContext dbContext)
  {
    m_RateTypeId = settings.Value.RateTypeId;
    m_DbContext= dbContext;
  }
  public void AddCurrencyRate(CurrencyRate rate) => m_DbContext.CurrencyRates.Add(rate);

  public async Task<IReadOnlyList<CurrencyRate>> GetCurrentState(DateTime first, DateTime last)
  {
    return await m_DbContext.CurrencyRates.Where(
      (e) => (
        e.ADate >= first
        && e.ADate <= last
        && e.RateTypesID == m_RateTypeId
        )
      ).ToListAsync();
  }

  public Task<int> SaveChangesAsync(CancellationToken token)
  {
    return m_DbContext.SaveChangesAsync(token);
  }
}
