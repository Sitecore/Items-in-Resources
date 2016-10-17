namespace Sitecore.Data.DataProviders
{
  using System.Linq;
  using Sitecore.Data;
  using Sitecore.Extensions;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Workflows;

  public partial class CompositeDataProvider
  {
    /* Workflow part of DataProvider */

    public override DataUri[] GetItemsInWorkflowState(WorkflowInfo info, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetItemsInWorkflowState(info, context));
    }

    public override WorkflowInfo GetWorkflowInfo(ItemDefinition item, VersionUri version, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetWorkflowInfo(item, version, context));
    }

    public override bool SetWorkflowInfo(ItemDefinition item, VersionUri version, WorkflowInfo info, CallContext context)
    {
      return Providers.Any(x => x.SetWorkflowInfo(item, version, info, context));
    }        
  }
}