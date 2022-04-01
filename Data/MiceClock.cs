namespace Aweton.Labs.CurrencyRates.Cli.Data;

internal class MiceClock : IMiceClock
{
  public DateTime GetUtcDate()
  {
    return DateTime.UtcNow;
  }
}