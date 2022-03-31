using Aweton.Labs.CurrencyRates.Cli.Data;
using Aweton.Labs.CurrencyRates.Cli.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aweton.Labs.CurrencyRates.Cli.Strategy;
internal class CurrencyLoader
{
  private readonly IServiceProvider m_Services;
  private readonly DateTime m_DefaultFirstDate;

  public CurrencyLoader(IServiceProvider services, IOptions<StarterSettings> settings)
  {
    m_Services = services;
    m_DefaultFirstDate = settings.Value.DefaultFirstDate;
  }

  public async Task Run()
  {
    await Register(await Fetch(await Initialize()));

  }

  private Task Register(IReadOnlyList<CurrencyRate> currencyRates)
  {
    throw new NotImplementedException();
  }

  private Task<IReadOnlyList<CurrencyRate>> Fetch((DateTime, IReadOnlyDictionary<string,int>) p)
  {
    throw new NotImplementedException();
  }

  private async Task<(DateTime, IReadOnlyDictionary<string,int>)> Initialize()
  {

    var dateTask = GetLastKnownFetchDate();
    var codesTask = GetCurrencyCodes();

    await Task.WhenAll(dateTask, codesTask);

    return (await dateTask, await codesTask);
  }

  private Task<IReadOnlyDictionary<string,int>> GetCurrencyCodes()
  {
    throw new NotImplementedException();
  }

  private async Task<DateTime> GetLastKnownFetchDate()
  {
    return await WithScope(
      async (IStarterDataWorker worker) =>
      {
        return await worker.GetLastKnownDateRun() ?? m_DefaultFirstDate;
      }
    );
  }

  private async Task<TResult> WithScope<TWorker, TResult>(Func<TWorker, Task<TResult>> callback) where TWorker : notnull
  {
    using var scope = m_Services.CreateScope();
    return await callback(scope.ServiceProvider.GetRequiredService<TWorker>());
  }
}