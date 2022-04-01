using Aweton.Labs.CurrencyRates.Cli.CbrXml;

namespace Aweton.Labs.CurrencyRates.Cli.Data;

public interface IValCursSerializer{
  ValCurs Deserialize(Stream stream);
}
