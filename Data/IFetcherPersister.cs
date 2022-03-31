using Aweton.Labs.CurrencyRates.Cli.Models;

namespace Aweton.Labs.CurrencyRates.Cli.Data;

public interface IFetcherPersister{
  Task<IReadOnlyList<CurrencyRate>> Load(DateTime after, IReadOnlyDictionary<int,string> currencyNameToIdMap);
}