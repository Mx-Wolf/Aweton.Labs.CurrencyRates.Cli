using Aweton.Labs.CurrencyRates.Cli.Data;
using Aweton.Labs.CurrencyRates.Cli.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aweton.Labs.CurrencyRates.Cli.Strategy;
internal class CurrencyLoader: ICurrencyLoader
{
  private readonly IServiceProvider m_Services;
  private readonly DateTime m_DefaultFirstDate;
  private readonly TimeSpan? m_RepeatLoopInterval;

  public CurrencyLoader(IServiceProvider services, IOptions<StarterSettings> settings)
  {
    m_Services = services;
    m_DefaultFirstDate = settings.Value.DefaultFirstDate;
    m_RepeatLoopInterval = settings.Value.RepeatLoopInterval;
  }

  public async Task Run()
  {
    while(true){
      await Register(await Fetch(await Initialize()));
      if(m_RepeatLoopInterval == null){
        return;
      }
      await Task.Delay(m_RepeatLoopInterval.Value);
    }

  }

  private Task Register(IReadOnlyList<CurrencyRate> currencyRates)
  {
    return WithScope( (IRegistarWorker worker)=>worker.Register(currencyRates));
  }

  private async Task<IReadOnlyList<CurrencyRate>> Fetch(DateTime after)
  {
    return await WithScope(async (IFetchWorker worker) => await worker.Load(after));
  }

  private async Task<DateTime> Initialize() => await GetLastKnownFetchDate();


  private async Task<DateTime> GetLastKnownFetchDate()
  {
    return await WithScope(
      async (IStarterDataWorker worker) => await worker.GetLastKnownDateRun() ?? m_DefaultFirstDate);
  }

  private async Task<TResult> WithScope<TWorker, TResult>(Func<TWorker, Task<TResult>> callback) where TWorker : notnull
  {
    using var scope = m_Services.CreateScope();
    return await callback(scope.ServiceProvider.GetRequiredService<TWorker>());
  }
}