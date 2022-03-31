using Aweton.Labs.CurrencyRates.Cli.Data;
using Microsoft.Extensions.Options;

namespace Aweton.Labs.CurrencyRates.Cli.Strategy;
internal class Starter
{
  private readonly IStarterDataWorker m_Persister;
  private readonly DateTime m_DefaultFirstDate;
  public Starter(IStarterDataWorker persister, IOptions<StarterSettings> settings)
  {
    m_Persister = persister;
    m_DefaultFirstDate = settings.Value.DefaultFirstDate;
  }

  public async Task<DateTime> GetFirstDate()
  {
    var lastKnown = await m_Persister.GetLastKnownDateRun();
    return lastKnown == null ? m_DefaultFirstDate : lastKnown.Value;
  }
}