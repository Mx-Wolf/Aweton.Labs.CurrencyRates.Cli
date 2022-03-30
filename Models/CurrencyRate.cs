namespace Aweton.Labs.CurrencyRates.Cli.Models
{
  public partial class CurrencyRate
    {
        public int CurrencyRatesID { get; set; }
        public int RateTypesID { get; set; }
        public int CurrencyTypesID { get; set; }
        public double Rate { get; set; }
        public DateTime ADate { get; set; }
        public string UserName { get; set; } = null!;
    }
}
