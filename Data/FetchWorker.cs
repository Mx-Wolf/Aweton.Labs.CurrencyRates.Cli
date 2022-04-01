using System.Globalization;
using Aweton.Labs.CurrencyRates.Cli.CbrXml;
using Aweton.Labs.CurrencyRates.Cli.Models;
using Microsoft.Extensions.Options;

namespace Aweton.Labs.CurrencyRates.Cli.Data;
internal class FetchWorker : IFetchWorker
{
  public const string HttpClientName = "CbrHttpClient";
  private const string _cultureName = "ru-RU";
  private readonly string m_Url;
  private readonly int m_RateTypesId;
  private readonly int m_LookAhead;
  private readonly IHttpClientFactory m_HttpClient;
  private readonly IMiceClock m_MiceClock;
  private readonly ICurrencyCodesWorker m_CodesWorker;
  private readonly IValCursSerializer m_ValCursSerializer;

  public FetchWorker(
    IOptions<FetchWorkerSettings> settings, 
    IOptions<MiceDbSettings> mice,
    IHttpClientFactory httpClient, 
    IMiceClock miceClock, 
    ICurrencyCodesWorker codesWorker, 
    IValCursSerializer valCursSerializer)
  {
    m_Url = settings.Value.Url;
    m_RateTypesId = mice.Value.RateTypesId;
    m_LookAhead = settings.Value.LookAhead;
    m_HttpClient = httpClient;
    m_MiceClock = miceClock;
    m_CodesWorker = codesWorker;
    m_ValCursSerializer = valCursSerializer;
  }

  public async Task<IReadOnlyList<CurrencyRate>> Load(DateTime after)
  {
    return await Dates(after).Aggregate(Task.FromResult(new List<CurrencyRate>()),CollectRatesFor) ;   
  }

  private async Task<List<CurrencyRate>> CollectRatesFor(Task<List<CurrencyRate>> accumulatorTask, DateTime date)
  {
    var accumulator = await accumulatorTask;
    var codes = await m_CodesWorker.Load();
    using var stream = await GetHttpResponse(date);
    return AddRangeFromStream(accumulator, stream, codes);
  }

  private List<CurrencyRate> AddRangeFromStream(List<CurrencyRate> accumulator, Stream stream, IReadOnlyDictionary<string, int> codes)
  {
    var curs = m_ValCursSerializer.Deserialize(stream);
    accumulator.AddRange(Transform(curs, codes));
    return accumulator;
  }

  private async Task<Stream> GetHttpResponse(DateTime date)
  {
    var response = await m_HttpClient.CreateClient(HttpClientName).GetAsync(FormatUrl(date));
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStreamAsync();
  }

  private IEnumerable<CurrencyRate> Transform(ValCurs curs, IReadOnlyDictionary<string, int> codes)
  {
    var aDate = ParseDate(curs);
    return curs.Valute.Aggregate(new List<CurrencyRate>(),(list,value)=>{
      if(codes.TryGetValue(value.CharCode,out int currencyTypesId)){
        list.Add(new CurrencyRate{
          ADate=aDate,
          CurrencyRatesID=0,
          CurrencyTypesID=currencyTypesId,
          Rate=ParseFloat(value.Value)/value.Nominal,
          RateTypesID=m_RateTypesId,
          UserName = $"inserted at {m_MiceClock.GetUtcDate().ToString("O")}"
        });
      }
      return list;
    });
  }

  private double ParseFloat(string value)
  {
    return double.Parse(value, NumberStyles.Any, CultureInfo.GetCultureInfo(_cultureName));
  }


  private DateTime ParseDate(ValCurs curs)
  {
    return DateTime.ParseExact(curs.Date,"dd.MM.yyyy",CultureInfo.GetCultureInfo(_cultureName));
  }

  private string? FormatUrl(DateTime date) => $"{m_Url}?date_req={date.Day:00}/{date.Month:00}/{date.Year:0000}";

  private IEnumerable<DateTime> Dates(DateTime after)
  {
    DateTime last = m_MiceClock.GetUtcDate().AddDays(m_LookAhead);
    while(after <= last)
    {
      after = after.AddDays(1);
      yield return after;
    }
  }
}
