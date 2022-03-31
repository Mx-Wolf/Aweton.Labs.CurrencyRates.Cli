
namespace Aweton.Labs.CurrencyRates.Cli.Data;
public interface IStarterDataWorker{
  Task<DateTime?> GetLastKnownDateRun();
}
