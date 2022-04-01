using Aweton.Labs.CurrencyRates.Cli.Models;

namespace Aweton.Labs.CurrencyRates.Cli.Data;

public interface IRegistarPersister{
  Task<int> SaveChangesAsync(CancellationToken token);
  void AddCurrencyRate(CurrencyRate rate);
  Task<IReadOnlyList<CurrencyRate>> GetCurrentState(DateTime first, DateTime last);
}

public interface IRegistarWorker
{
  Task<int> Register(IReadOnlyList<CurrencyRate> fetched);
}