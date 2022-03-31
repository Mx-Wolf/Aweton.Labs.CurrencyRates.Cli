using Aweton.Labs.CurrencyRates.Cli.Models;
using Microsoft.Extensions.Options;

namespace Aweton.Labs.CurrencyRates.Cli.Data;

public interface IFetchWorker
{
  Task<IReadOnlyList<CurrencyRate>> Load(DateTime after, IReadOnlyDictionary<int, string> currencyNameToIdMap);
}
public class FetchWorkerSettins
{
  public string Url { get; set; } = null!;
}
internal class FetchWorker : IFetchWorker
{
  private readonly string m_Url;
  private readonly HttpClient m_HttpClient;

  public FetchWorker(IOptions<FetchWorkerSettins> settings, HttpClient httpClient)
  {
    m_Url = settings.Value.Url;
    m_HttpClient = httpClient;
  }

  public Task<IReadOnlyList<CurrencyRate>> Load(DateTime after, IReadOnlyDictionary<int, string> currencyNameToIdMap)
  {
    throw new NotImplementedException();
  }
}
