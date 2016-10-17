namespace Sitecore.Data.DataProviders
{
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Extensions.Enumerable;

  public partial class CompositeDataProvider
  {
    /* Items.ItemTree part of DataProvider */

    public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetItemDefinition(itemId, context));
    }

    public override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetParentID(itemDefinition, context));
    }

    public override bool HasChildren(ItemDefinition itemDefinition, CallContext context)
    {
      return Providers.Any(x => x.HasChildren(itemDefinition, context));
    }

    public override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetChildIDs(itemDefinition, context));
    }

    public override ID ResolvePath(string itemPath, CallContext context)
    {
      return Providers.FirstNotNull(x => x.ResolvePath(itemPath, context));
    }

    public override IDList SelectIDs(string query, CallContext context)
    {
      return Providers.FirstNotNull(x => x.SelectIDs(query, context));
    }

    public override ID SelectSingleID(string query, CallContext context)
    {
      return Providers.FirstNotNull(x => x.SelectSingleID(query, context));
    }
  }
}