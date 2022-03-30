
using Microsoft.EntityFrameworkCore;

namespace Aweton.Labs.CurrencyRates.Cli.Data;

internal class StarterPersister : IStarterPersister
{
  private readonly MiceDbContext m_DbContext ;
  public StarterPersister(MiceDbContext dbContext)
  {
    m_DbContext=dbContext;
  }
  public Task<DateTime?> GetLastKnownDateRun()
  {
    return m_DbContext.awjCbrDailyLogs.MaxAsync((e)=>(DateTime?)e.aDate);
  }
}
