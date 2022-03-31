using Aweton.Labs.CurrencyRates.Cli.Models;

namespace Aweton.Labs.CurrencyRates.Cli.Data;

public interface IFetchWorker
{
  Task<IReadOnlyList<CurrencyRate>> Load(DateTime after);
}
