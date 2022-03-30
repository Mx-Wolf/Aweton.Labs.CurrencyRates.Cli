using Aweton.Labs.CurrencyRates.Cli.Models;
using Microsoft.EntityFrameworkCore;

namespace Aweton.Labs.CurrencyRates.Cli.Data;

partial class MiceDbContext : IMiceDbContext
{
  private readonly int m_RateTypeId;
  public void AddCurrencyRate(CurrencyRate rate) => CurrencyRates.Add(rate);

  public async Task<IReadOnlyList<CurrencyRate>> GetCurrentState(DateTime first, DateTime last)
  {
    return await CurrencyRates.Where(
      (e) => (
        e.ADate >= first
        && e.ADate <= last
        && e.RateTypesID == m_RateTypeId
        )
      ).ToListAsync();
  }

  
}
