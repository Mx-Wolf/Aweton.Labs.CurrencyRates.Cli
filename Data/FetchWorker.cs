using System.Xml.Serialization;
using Aweton.Labs.CurrencyRates.Cli.CbrXml;
using Aweton.Labs.CurrencyRates.Cli.Models;
using Microsoft.Extensions.Options;

namespace Aweton.Labs.CurrencyRates.Cli.Data;
public interface IMiceClock{
  DateTime GetDate();
}
internal class FetchWorker : IFetchWorker
{
  private readonly string m_Url;
  private readonly int m_RateTypesId;
  private readonly int m_LookAhead;
  private readonly HttpClient m_HttpClient;
  private readonly IMiceClock m_MiceClock;
  private readonly ICurrencyCodesWorker m_CodesWorker;

  public FetchWorker(IOptions<FetchWorkerSettings> settings, HttpClient httpClient, IMiceClock miceClock, ICurrencyCodesWorker codesWorker)
  {
    m_Url = settings.Value.Url;
    m_RateTypesId = settings.Value.RateTypesId;
    m_LookAhead = settings.Value.LookAhead;
    m_HttpClient = httpClient;
    m_MiceClock = miceClock;
    m_CodesWorker = codesWorker;
  }

  public async Task<IReadOnlyList<CurrencyRate>> Load(DateTime after)
  {
    return await Dates(after).Aggregate(Task.FromResult(new List<CurrencyRate>()),CollectRatesFor) ;   
  }

  private async Task<List<CurrencyRate>> CollectRatesFor(Task<List<CurrencyRate>> accumulatorTask, DateTime date)
  {
    var response = await m_HttpClient.GetAsync(FormatUrl(date));
    response.EnsureSuccessStatusCode();
    using var stream = await response.Content.ReadAsStreamAsync();
    var serializer = new XmlSerializer(typeof (ValCurs));
    var curs = (ValCurs?)serializer.Deserialize(stream);
    if(curs == null){
      throw new InvalidDataException();
    }
    var accumulator = await accumulatorTask;
    accumulator.AddRange(Transform(curs));
    return accumulator;
  }

  private IEnumerable<CurrencyRate> Transform(ValCurs curs)
  {
    throw new NotImplementedException();
  }

  private string? FormatUrl(DateTime date) => $"{m_Url}?date_req={date.Day:00}/{date.Month:00}/{date.Year:0000}";

  private IEnumerable<DateTime> Dates(DateTime after)
  {
    DateTime last = m_MiceClock.GetDate().AddDays(m_LookAhead);
    while(after <= last)
    {
      after = after.AddDays(1);
      yield return after;
    }
  }
}
