namespace Sitecore.Data.DataProviders
{
  using Sitecore.Eventing;
  using System.Collections.Generic;
  using Sitecore.Diagnostics;

  public partial class CompositeDataProvider
  {
    /* Properties part of DataProvider */

    public override string GetProperty(string name, CallContext context)
    {
      return HeadProvider?.GetProperty(name, context);
    }

    public override List<string> GetPropertyKeys(string prefix, CallContext context)
    {
      return HeadProvider?.GetPropertyKeys(prefix, context) ?? new List<string>();
    }

    public override bool SetProperty(string name, string value, CallContext context)
    {
      return HeadProvider?.SetProperty(name, value, context) ?? false;
    }

    public override bool RemoveProperty(string name, bool isPrefix, CallContext context)
    {
      return HeadProvider?.RemoveProperty(name, isPrefix, context) ?? false;
    }

    protected override EventQueue DoGetEventQueue()
    {
      var headProvider = HeadProvider;
      Assert.IsNotNull(headProvider, $"{nameof(headProvider)} is null");

      return headProvider.GetEventQueue();
    }
  }
}