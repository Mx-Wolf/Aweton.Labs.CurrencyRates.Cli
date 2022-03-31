namespace Aweton.Labs.CurrencyRates.Cli.Data;

public class FetchWorkerSettings
{
  public string Url { get; set; } = null!;
  public int RateTypesId {get;set;}
  public int LookAhead {get;set;}
}
