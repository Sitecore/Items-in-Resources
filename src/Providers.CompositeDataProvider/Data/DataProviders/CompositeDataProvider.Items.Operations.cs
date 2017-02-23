namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Diagnostics;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Extensions.Object;
  using Sitecore.SecurityModel;

  public partial class CompositeDataProvider
  {
    /* Items.ItemTree part of DataProvider */

    public override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, DateTime created, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var isCreated = HeadProvider?.CreateItem(itemID, itemName, templateID, parent, created, context) ?? false;

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

      var isCreated = HeadProvider?.CreateItem(itemID, itemName, templateID, parent, context) ?? false;

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

      if (HeadProvider?.GetItemDefinition(itemDefinition.ID, context) == null)
      {
        var item = changes.Item;
        Assert.IsNotNull(item, nameof(item));

        if (!MigrateDefaultItem(itemDefinition, item, context))
        {
          return false;
        }
      }

      var saved = HeadProvider?.SaveItem(itemDefinition, changes, context) ?? false;
                                                                           
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
      if (HeadProvider?.GetItemDefinition(source.ID, context) != null)
      {
        if (HeadProvider?.CopyItem(source, destination, copyName, copyID, context) ?? false)
        {
          return true;
        }
      }

      var database = context.DataManager.Database;
      var itemId = source.ID;

      using (new SecurityDisabler())
      {
        var item = database.GetItem(itemId);
        Assert.IsNotNull(item, nameof(item));

        using (var limit = new RecursionLimit($"{nameof(CopyItem)}-{item.ID}-{destination.ID}", 1))
        {
          if (limit.Exceeded)
          {
            return true;
          }

          var defaultOptions = ItemSerializerOptions.GetDefaultOptions();
          defaultOptions.AllowDefaultValues = false; // TODO: needs checking
          defaultOptions.AllowStandardValues = false;
          defaultOptions.IncludeBlobFields = true;
          defaultOptions.ProcessChildren = true; // TODO: slow, needs optimization
          var outerXml = item.GetOuterXml(defaultOptions);

          var target = database.GetItem(destination.ID);
          Assert.IsNotNull(target, nameof(target));

          target.Paste(outerXml, true, PasteMode.Overwrite);
          Log.Audit(this, "Default item {0} ({1}) was copied to {2} in head provider", item.Name, item.ID.ToString(), destination.ID.ToString());

          return true;
        }
      }
    }

    public override bool MoveItem([NotNull] ItemDefinition itemDefinition, [NotNull] ItemDefinition destination, [NotNull] CallContext context)
    {
      Assert.ArgumentNotNull(itemDefinition, nameof(itemDefinition));
      Assert.ArgumentNotNull(destination, nameof(destination));
      Assert.ArgumentNotNull(context, nameof(context));

#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      if (HeadProvider?.GetItemDefinition(itemDefinition.ID, context) == null)
      {
        using (new SecurityDisabler())
        {
          var item = context.DataManager.Database.GetItem(itemDefinition.ID);
          Assert.IsNotNull(item, nameof(item));

          if (!MigrateDefaultItem(itemDefinition, item, context))
          {
#if DEBUG
            this.Trace(false, timer, itemDefinition.ID, destination.ID, context);
#endif

            return false;
          }
        }
      }

      var moved = HeadProvider?.MoveItem(itemDefinition, destination, context) ?? false;

#if DEBUG
      this.Trace(moved, timer, itemDefinition.ID, destination.ID, context);
#endif

      return moved;
    }

    public override bool DeleteItem(ItemDefinition itemDefinition, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      // check if already deleted in head
      var headParentId = HeadProvider?.GetParentID(itemDefinition, context);
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

        var deleted = HeadProvider?.DeleteItem(itemDefinition, context) ?? false;

#if DEBUG
        this.Trace(deleted, timer, itemDefinition.ID, context);
#endif
        return deleted;
      }

      if (HeadProvider?.GetItemDefinition(itemId, context) != null)
      {
        // item exists both in read-only data provider and in HEAD
        // so we first delete it in HEAD

        HeadProvider?.DeleteItem(itemDefinition, context);

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

        using (new SecurityDisabler())
        {
          var defaultOptions = ItemSerializerOptions.GetDefaultOptions();
          defaultOptions.AllowDefaultValues = false;
          defaultOptions.AllowStandardValues = false;
          defaultOptions.IncludeBlobFields = true;
          defaultOptions.ProcessChildren = false;
          var outerXml = item.GetOuterXml(defaultOptions);

          var parent = item.Parent;
          Assert.IsNotNull(parent, nameof(parent));

          parent.Paste(outerXml, false, PasteMode.Overwrite);
          Log.Audit(this, "Default item {0} ({1}) was migrated to head provider", item.Name, item.ID.ToString());

          var result = MoveItem(itemDefinition, GetItemDefinition(parent.ID, context), context);

          if (!result)
          {
            Log.Error($"Cannot migrate default item {item.Name} ({item.ID}) to head data provider", this);
          }
          else
          {
            parent.Database.Caches.DataCache.RemoveItemInformation(item.ID);
          }

          return result;
        }
      }
    }
  }
}