using System.Diagnostics.CodeAnalysis;
using Aweton.Labs.CurrencyRates.Cli.Models;

namespace Aweton.Labs.CurrencyRates.Cli.Strategy;
public class RateNaturalKeyComparer : IEqualityComparer<CurrencyRate>
{
  public bool Equals(CurrencyRate? x, CurrencyRate? y)
  {
    if(x== null && y == null){
      return true;
    }
    if(x==null || y == null){
      return false;
    }
    return x.ADate==y.ADate
    && x.CurrencyTypesID == y.CurrencyTypesID
    && x.RateTypesID == y.RateTypesID;
  }

  public int GetHashCode([DisallowNull] CurrencyRate obj)
  {
    return obj.ADate.GetHashCode()
    ^ obj.CurrencyTypesID.GetHashCode()
    ^ obj.RateTypesID.GetHashCode();
  }
}