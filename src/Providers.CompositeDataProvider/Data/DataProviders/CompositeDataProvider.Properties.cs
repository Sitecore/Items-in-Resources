namespace Sitecore.Data.DataProviders
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Extensions;

  public partial class CompositeDataProvider
  {
    /* Properties part of DataProvider */

    public override string GetProperty(string name, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetProperty(name, context));
    }

    public override List<string> GetPropertyKeys(string prefix, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetPropertyKeys(prefix, context));
    }

    public override bool SetProperty(string name, string value, CallContext context)
    {
      return Providers.Any(x => x.SetProperty(name, value, context));
    }

    public override bool RemoveProperty(string name, bool isPrefix, CallContext context)
    {
      return Providers.Any(x => x.RemoveProperty(name, isPrefix, context));
    }
  }
}