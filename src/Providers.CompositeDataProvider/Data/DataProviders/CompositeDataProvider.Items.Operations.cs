namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Linq;
  using Sitecore.Data;

  public partial class CompositeDataProvider
  {
    /* Items.ItemTree part of DataProvider */

    public override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, DateTime created, CallContext context)
    {
      return Providers.Any(x => x.CreateItem(itemID, itemName, templateID, parent, created, context));
    }

    public override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, CallContext context)
    {
      return Providers.Any(x => x.CreateItem(itemID, itemName, templateID, parent, context));
    }

    public override bool CopyItem(ItemDefinition source, ItemDefinition destination, string copyName, ID copyID, CallContext context)
    {
      return Providers.Any(x => x.CopyItem(source, destination, copyName, copyID, context));
    }

    public override bool MoveItem(ItemDefinition itemDefinition, ItemDefinition destination, CallContext context)
    {
      return Providers.Any(x => x.MoveItem(itemDefinition, destination, context));
    }         

    public override bool DeleteItem(ItemDefinition itemDefinition, CallContext context)
    {
      return Providers.Any(x => x.DeleteItem(itemDefinition, context));
    }
  }
}