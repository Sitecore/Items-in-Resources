namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Data.Items;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Extensions.Object;

  public partial class CompositeDataProvider
  {
    /* Items.ItemTree part of DataProvider */

    public override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, DateTime created, CallContext context)
    {
      var isCreated = HeadProvider.CreateItem(itemID, itemName, templateID, parent, created, context);

      this.Trace(isCreated, null, itemID, itemName, templateID, parent.ID, created, context.DataManager.Database.Name);

      return isCreated;
    }

    public override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, CallContext context)
    {
      var isCreated = HeadProvider.CreateItem(itemID, itemName, templateID, parent, context);

      this.Trace(isCreated, null, itemID, itemName, templateID, parent.ID, context.DataManager.Database.Name);

      return isCreated;
    }

    public override bool SaveItem(ItemDefinition itemDefinition, ItemChanges changes, CallContext context)
    {
      if (HeadProvider.GetItemDefinition(itemDefinition.ID, new CallContext(context.DataManager, 1)) != null)
      {
        var isSaved = HeadProvider.SaveItem(itemDefinition, changes, context);

        this.Trace(isSaved, null, itemDefinition.ID, context.DataManager.Database.Name);
        
        return isSaved;
      }

      var parentId = GetParentID(itemDefinition, context);
      var parentItem = GetItemDefinition(parentId, context);

      if (
        !HeadProvider.CreateItem(
          itemDefinition.ID,
          itemDefinition.Name,
          itemDefinition.TemplateID,
          parentItem,
          itemDefinition.Created,
          context))
      {             
        this.Trace(false, null, itemDefinition.ID, context.DataManager.Database.Name);

        return false;
      }

      foreach (VersionUri version in GetItemVersions(itemDefinition, context))
      {
        var versionFields = this.GetItemFields(itemDefinition, version, context);
        var versionCopy = new ItemChanges(changes.Item);
        foreach (KeyValuePair<ID, string> pair in versionFields)
        {
          versionCopy.SetFieldValue(versionCopy.Item.Fields[pair.Key], pair.Value);
        }

        HeadProvider.SaveItem(itemDefinition, versionCopy, context);
      }

      var saved = HeadProvider.SaveItem(itemDefinition, changes, context);

      this.Trace(saved, null, itemDefinition.ID, context.DataManager.Database.Name);
                   
      return saved;
    }

    public override bool CopyItem(ItemDefinition source, ItemDefinition destination, string copyName, ID copyID, CallContext context)
    {
      // source item is in head provider
      if (HeadProvider.CopyItem(source, destination, copyName, copyID, context))
      {
        this.Trace(true, null, source.ID, destination.ID, copyName, copyID, context.DataManager.Database.Name);

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
        this.Trace(true, null, itemDefinition.ID, destination.ID, context.DataManager.Database.Name);

        return true;
      }

      throw new NotImplementedException();
    }

    public override bool DeleteItem(ItemDefinition itemDefinition, CallContext context)
    {
      // check if already deleted in head
      var headParentId = HeadProvider.GetParentID(itemDefinition, context);
      if (headParentId == ID.Undefined)
      {
        this.Trace(true, null, itemDefinition.ID, context.DataManager.Database.Name);

        return true;
      }

      var itemId = itemDefinition.ID;
      if (ReadOnlyProviders.FirstNotNull(x => x.GetItemDefinition(itemId)) == null)
      {
        // item may only exist in head provider
        // so we can simply delete it 

        var deleted = HeadProvider.DeleteItem(itemDefinition, context);

        this.Trace(deleted, null, itemDefinition.ID, context.DataManager.Database.Name);

        return deleted;
      }

      if (HeadProvider.GetItemDefinition(itemId, context) != null)
      {
        // item exists both in read-only data provider and in HEAD
        // so we first delete it in HEAD

        HeadProvider.DeleteItem(itemDefinition, context);

        // and pretend it was only in read-only data provider
      }

      // item only exists in read-only data provider 
      // so we create item definition beneath undefied parent

      var deleted2 = CreateItem(itemId, itemDefinition.Name, itemDefinition.TemplateID, new ItemDefinition(ID.Undefined, "undefined", ID.Null, ID.Null), context);

      this.Trace(deleted2, null, itemDefinition.ID, context.DataManager.Database.Name);

      return deleted2;
    }
  }
}