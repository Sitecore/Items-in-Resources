namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Collections;
  using Sitecore.Publishing.Pipelines.Publish;

  public partial class CompositeDataProvider
  {
    /* PublishQueue part of DataProvider */

    public override bool AddToPublishQueue(ID itemID, string action, DateTime date, CallContext context)
    {
      return HeadProvider?.AddToPublishQueue(itemID, action, date, context) ?? false;
    }

    public override bool AddToPublishQueue(ID itemID, string action, DateTime date, string language, CallContext context)
    {
      return HeadProvider?.AddToPublishQueue(itemID, action, date, language, context) ?? false;
    }

    public override bool CleanupPublishQueue(DateTime to, CallContext context)
    {
      return HeadProvider?.CleanupPublishQueue(to, context) ?? false;
    }

    public override IDList GetPublishQueue(DateTime from, DateTime to, CallContext context)
    {
      return HeadProvider?.GetPublishQueue(from, to, context) ?? new IDList();
    }

    public override List<PublishQueueEntry> GetPublishQueueEntries(DateTime from, DateTime to, CallContext context)
    {
      return HeadProvider?.GetPublishQueueEntries(from, to, context) ?? new List<PublishQueueEntry>();
    }
  }
}