namespace Sitecore.Data.DataProviders
{
  using System;

  public partial class CompositeDataProvider
  {
    /* Items.ItemTree part of DataProvider */

    public override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, DateTime created, CallContext context)
    {
      return HeadProvider.CreateItem(itemID, itemName, templateID, parent, created, context);
    }

    public override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, CallContext context)
    {
      return HeadProvider.CreateItem(itemID, itemName, templateID, parent, context);
    }

    public override bool CopyItem(ItemDefinition source, ItemDefinition destination, string copyName, ID copyID, CallContext context)
    {
      // source item is in head provider
      if (HeadProvider.CopyItem(source, destination, copyName, copyID, context))
      {
        return true;
      }

      throw new NotImplementedException();

      /*
      var database = context.DataManager.Database;
      var itemId = source.ID;
      var sourceItem = database.GetItem(itemId);
      var sourceVersions = sourceItem.Versions.GetVersions(true);
      if (!CreateItem(itemId, copyName, sourceItem.TemplateID, destination, context))
      {
        return false;
      }

      var copyItem = database.GetItem(itemId);
      copyItem.Versions.RemoveAll(true);
      foreach (var languageGroup in sourceVersions.GroupBy(x => x.Language.Name))
      {
        var language = languageGroup.First().Language;
        var lastVersion = languageGroup.Max(x => x.Version.Number);
        for (var i = 1; i <= lastVersion; ++i)
        {
          copyItem = copyItem.Versions.AddVersion();
          var sourceVersion = sourceVersions.FirstOrDefault(x => x.Language == language && x.Version.Number == i);
        }
      }*/
    }

    public override bool MoveItem(ItemDefinition itemDefinition, ItemDefinition destination, CallContext context)
    {
      if (HeadProvider.MoveItem(itemDefinition, destination, context))
      {
        return true;
      }

      throw new NotImplementedException();
    }

    public override bool DeleteItem(ItemDefinition itemDefinition, CallContext context)
    {
      if (HeadProvider.DeleteItem(itemDefinition, context))
      {
        return true;
      }

      return CreateItem(itemDefinition.ID, itemDefinition.Name, itemDefinition.TemplateID, new ItemDefinition(ID.Undefined, "undefined", ID.Null, ID.Null), context);
    }
  }
}