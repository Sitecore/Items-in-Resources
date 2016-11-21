namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Extensions.Object;

  public partial class CompositeDataProvider
  {
    /* Items.ItemTree part of DataProvider */

    public override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, DateTime created, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var isCreated = HeadProvider.CreateItem(itemID, itemName, templateID, parent, created, context);

#if DEBUG
      this.Trace(isCreated, timer, itemID, itemName, templateID, parent.ID, created, context);
#endif

      return isCreated;
    }

    public override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var isCreated = HeadProvider.CreateItem(itemID, itemName, templateID, parent, context);

#if DEBUG
      this.Trace(isCreated, timer, itemID, itemName, templateID, parent.ID, context);
#endif

      return isCreated;
    }

    public override bool SaveItem([NotNull] ItemDefinition itemDefinition, [NotNull] ItemChanges changes, [NotNull] CallContext context)
    {
      Assert.ArgumentNotNull(itemDefinition, nameof(itemDefinition));
      Assert.ArgumentNotNull(changes, nameof(changes));
      Assert.ArgumentNotNull(context, nameof(context));

#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      if (HeadProvider.GetItemDefinition(itemDefinition.ID, context) == null)
      {
        var item = changes.Item;
        Assert.IsNotNull(item, nameof(item));

        if (!MigrateDefaultItem(itemDefinition, item, context))
        {
          Log.Error($"Cannot migrate default item {item.Name} ({item.ID}) to head data provider", this);

          return false;
        }
      }                                                  

      var saved = HeadProvider.SaveItem(itemDefinition, changes, context);

#if DEBUG
      this.Trace(saved, timer, itemDefinition.ID, context);
#endif
                   
      return saved;
    }

    public override bool CopyItem(ItemDefinition source, ItemDefinition destination, string copyName, ID copyID, CallContext context)
    {
#if DEBUG
      this.Trace(true, null, source.ID, destination.ID, copyName, copyID, context);
#endif

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

#if DEBUG
      this.Trace(true, null, itemDefinition.ID, destination.ID, context);
#endif   

      if (HeadProvider.MoveItem(itemDefinition, destination, context))
      {
        return true;
      }

      throw new NotImplementedException();
    }

    public override bool DeleteItem(ItemDefinition itemDefinition, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      // check if already deleted in head
      var headParentId = HeadProvider.GetParentID(itemDefinition, context);
      if (headParentId == ID.Undefined)
      {
#if DEBUG
        this.Trace(true, timer, itemDefinition.ID, context);
#endif
        return true;
      }

      var itemId = itemDefinition.ID;
      if (ReadOnlyProviders.FirstNotNull(x => x.GetItemDefinition(itemId)) == null)
      {
        // item may only exist in head provider
        // so we can simply delete it 

        var deleted = HeadProvider.DeleteItem(itemDefinition, context);

#if DEBUG
        this.Trace(deleted, timer, itemDefinition.ID, context);
#endif
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

#if DEBUG
      this.Trace(deleted2, timer, itemDefinition.ID, context);
#endif

      return deleted2;
    }

    private bool MigrateDefaultItem([NotNull] ItemDefinition itemDefinition, [NotNull] Item item, [NotNull] CallContext context)
    {
      Assert.ArgumentNotNull(itemDefinition, nameof(itemDefinition));
      Assert.ArgumentNotNull(item, nameof(item));
      Assert.ArgumentNotNull(context, nameof(context));

      using (var limit = new RecursionLimit($"{nameof(MigrateDefaultItem)}-{item.ID}", 1))
      {
        if (limit.Exceeded)
        {
          return true;
        }

        var defaultOptions = ItemSerializerOptions.GetDefaultOptions();
        defaultOptions.AllowDefaultValues = false;
        defaultOptions.AllowStandardValues = false;
        defaultOptions.IncludeBlobFields = true;
        defaultOptions.ProcessChildren = false;
        var outerXml = item.GetOuterXml(defaultOptions);

        var parent = item.Parent;
        Assert.IsNotNull(parent, nameof(parent));

        parent.Paste(outerXml, false, PasteMode.Overwrite);
        Log.Audit(this, $"Default item {item.Name} ({item.Paths.FullPath}) was migrated to head provider");

        return MoveItem(itemDefinition, GetItemDefinition(parent.ID, context), context);
      }
    }
  }
}