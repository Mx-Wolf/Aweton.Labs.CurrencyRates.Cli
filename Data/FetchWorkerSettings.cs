namespace Aweton.Labs.CurrencyRates.Cli.Data;

public class FetchWorkerSettings
{
  public string Url { get; set; } = null!;  
  public int LookAhead {get;set;}
}
