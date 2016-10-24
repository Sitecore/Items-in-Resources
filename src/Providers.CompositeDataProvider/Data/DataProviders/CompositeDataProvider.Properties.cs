namespace Sitecore.Data.DataProviders
{
  using Sitecore.Eventing;
  using System.Collections.Generic;

  public partial class CompositeDataProvider
  {
    /* Properties part of DataProvider */

    public override string GetProperty(string name, CallContext context)
    {
      return HeadProvider.GetProperty(name, context);
    }

    public override List<string> GetPropertyKeys(string prefix, CallContext context)
    {
      return HeadProvider.GetPropertyKeys(prefix, context);
    }

    public override bool SetProperty(string name, string value, CallContext context)
    {
      return HeadProvider.SetProperty(name, value, context);
    }

    public override bool RemoveProperty(string name, bool isPrefix, CallContext context)
    {
      return HeadProvider.RemoveProperty(name, isPrefix, context);
    }

    protected override EventQueue DoGetEventQueue()
    {
      return HeadProvider.GetEventQueue();
    }
  }
}