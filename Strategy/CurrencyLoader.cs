using Aweton.Labs.CurrencyRates.Cli.Models;

namespace Aweton.Labs.CurrencyRates.Cli.Strategy;
internal class CurrencyLoader
{  
  public async Task Run()
  {
    await Register(await Fetch(await Initialize()));

  }

  private Task Register(IReadOnlyList<CurrencyRate> currencyRates)
  {
    throw new NotImplementedException();
  }

  private Task<IReadOnlyList<CurrencyRate>> Fetch((DateTime, IReadOnlyDictionary<int, string>) p)
  {
    throw new NotImplementedException();
  }

  private async Task<(DateTime, IReadOnlyDictionary<int, string>)> Initialize()
  {
    
    var dateTask =  GetLastKnownFetchDate();
    var codesTask =  GetCurrencyCodes();

    await Task.WhenAll(dateTask,codesTask);
    
    return (await dateTask, await codesTask);
  }

  private Task<IReadOnlyDictionary<int,string>> GetCurrencyCodes()
  {
    throw new NotImplementedException();
  }

  private Task<DateTime> GetLastKnownFetchDate()
  {
    throw new NotImplementedException();
  }
}