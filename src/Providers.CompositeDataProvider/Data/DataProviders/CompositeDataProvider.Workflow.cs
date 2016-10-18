namespace Sitecore.Data.DataProviders
{
  using Sitecore.Workflows;

  public partial class CompositeDataProvider
  {
    /* Workflow part of DataProvider */

    public override DataUri[] GetItemsInWorkflowState(WorkflowInfo info, CallContext context)
    {
      return HeadProvider.GetItemsInWorkflowState(info, context);
    }

    public override WorkflowInfo GetWorkflowInfo(ItemDefinition item, VersionUri version, CallContext context)
    {
      return HeadProvider.GetWorkflowInfo(item, version, context);
    }

    public override bool SetWorkflowInfo(ItemDefinition item, VersionUri version, WorkflowInfo info, CallContext context)
    {
      return HeadProvider.SetWorkflowInfo(item, version, info, context);
    }
  }
}