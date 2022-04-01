namespace Aweton.Labs.CurrencyRates.Cli.Data;

public interface ICurrencyCodesWorker
{
  Task<IReadOnlyDictionary<string, int>> Load();

}
