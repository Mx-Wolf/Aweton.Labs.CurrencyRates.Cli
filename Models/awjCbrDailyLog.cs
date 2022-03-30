namespace Aweton.Labs.CurrencyRates.Cli.Models
{
  public partial class awjCbrDailyLog
    {
        public int awjCbrDailyLogID { get; set; }
        public DateTime aDate { get; set; }
        public int? Rows { get; set; }
        public byte[]? HashBytes { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
