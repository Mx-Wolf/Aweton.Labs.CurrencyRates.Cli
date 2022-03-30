
namespace Aweton.Labs.CurrencyRates.Cli.Data;
public interface IStarterPersister{
  Task<DateTime?> GetLastKnownDateRun();
}
