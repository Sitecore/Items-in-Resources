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
      return HeadProvider.AddToPublishQueue(itemID, action, date, context);
    }

    public override bool AddToPublishQueue(ID itemID, string action, DateTime date, string language, CallContext context)
    {
      return HeadProvider.AddToPublishQueue(itemID, action, date, language, context);
    }

    public override bool CleanupPublishQueue(DateTime to, CallContext context)
    {
      return HeadProvider.CleanupPublishQueue(to, context);
    }

    public override IDList GetPublishQueue(DateTime from, DateTime to, CallContext context)
    {
      return HeadProvider.GetPublishQueue(from, to, context);
    }

    public override List<PublishQueueEntry> GetPublishQueueEntries(DateTime from, DateTime to, CallContext context)
    {
      return HeadProvider.GetPublishQueueEntries(from, to, context);
    }
  }
}