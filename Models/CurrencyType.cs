namespace Aweton.Labs.CurrencyRates.Cli.Models
{
  public partial class CurrencyType
    {
        public int CurrencyTypesID { get; set; }
        public string CurrCodeInt { get; set; } = null!;
        public string CurrCodeChr { get; set; } = null!;
        public string Description { get; set; } = null!;
        public short? bInverse { get; set; }
        public string? Eng_description { get; set; }
        public string? Case_1 { get; set; }
        public string? Case_2 { get; set; }
        public string? Case_5 { get; set; }
        public string? Case1 { get; set; }
        public string? Case2 { get; set; }
        public string? Case5 { get; set; }
        public int? CaseMale { get; set; }
    }
}
