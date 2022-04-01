using System.Xml.Serialization;
using Aweton.Labs.CurrencyRates.Cli.CbrXml;

namespace Aweton.Labs.CurrencyRates.Cli.Data;

internal class ValCursSerializer : IValCursSerializer
{
  private readonly XmlSerializer m_Serializer = new XmlSerializer(typeof(ValCurs));
  public ValCurs Deserialize(Stream stream)
  {
    var result = m_Serializer.Deserialize(stream);
    if(result == null){
      throw new ArgumentException(nameof(stream));
    }
    return (ValCurs) result;
  }
}
