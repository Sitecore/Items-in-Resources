namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Extensions;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Publishing.Pipelines.Publish;
  
  public partial class CompositeDataProvider
  {
    /* PublishQueue part of DataProvider */

    public override bool AddToPublishQueue(ID itemID, string action, DateTime date, CallContext context)
    {
      return Providers.Any(x => x.AddToPublishQueue(itemID, action, date, context));
    }

    public override bool AddToPublishQueue(ID itemID, string action, DateTime date, string language, CallContext context)
    {
      return Providers.Any(x => x.AddToPublishQueue(itemID, action, date, language, context));
    }

    public override bool CleanupPublishQueue(DateTime to, CallContext context)
    {
      return Providers.Any(x => x.CleanupPublishQueue(to, context));
    }

    public override IDList GetPublishQueue(DateTime from, DateTime to, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetPublishQueue(from, to, context));
    }

    public override List<PublishQueueEntry> GetPublishQueueEntries(DateTime from, DateTime to, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetPublishQueueEntries(from, to, context));
    }
  }
}