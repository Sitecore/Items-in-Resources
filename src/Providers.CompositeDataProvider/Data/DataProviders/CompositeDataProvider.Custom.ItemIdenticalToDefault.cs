namespace Sitecore.Data.DataProviders
{
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Extensions.Database;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.SecurityModel;

  partial class CompositeDataProvider
  {
    public bool TryRemoveItemData(ID itemId, bool recurse = false)
    {
      using (var transaction = HeadProviderEx.OpenTransaction())
      {
        using (new SecurityDisabler())
        {
          var result = TryRemoveItemDataInner(itemId, recurse);

          transaction.Complete();

          return result;
        }        
      }
    }

    private bool TryRemoveItemDataInner(ID itemId, bool recurse)
    {
      var itemDefinition = new ItemDefinition(itemId, string.Empty, ID.Undefined, ID.Undefined);
      var childrenIds = recurse ? GetChildrenIds(itemDefinition) : new ID[0];

      var result = false;
      if (CanBeRemovedFromHead(itemId))
      {
        HeadProviderEx.RemoveItemData(itemId);
        Database.RemoveFromCaches(itemId);

        result = true;
      }

      foreach (var childId in childrenIds)
      {
        result |= TryRemoveItemDataInner(childId, recurse);
      }

      return result;
    }

    private ID[] GetChildrenIds(ItemDefinition itemDefinition)
    {
      var dataManager = Database.DataManager;
      var childrenIds = GetChildIDs(itemDefinition, new CallContext(dataManager, 1)).Cast<ID>().ToArray();

      return childrenIds;
    }

    public bool CanBeRemovedFromHead([NotNull] ID itemId)
    {
      Assert.ArgumentNotNull(itemId, nameof(itemId));

      var dataManager = Database.DataManager;
      if (HeadProvider.GetItemDefinition(itemId, new CallContext(dataManager, 1)) == null)
      {
        return false;
      }

      if (ReadOnlyProviders.All(x => x.GetItemDefinition(itemId) == null))
      {
        return false;
      }

      var itemDefinition = new ItemDefinition(itemId, string.Empty, ID.Undefined, ID.Undefined);
      foreach (VersionUri version in GetItemVersions(itemDefinition, new CallContext(dataManager, 1)))
      {
        var versionUri = new VersionUri(version.Language, version.Version);

        var actualFields = HeadProvider.GetItemFields(itemDefinition, versionUri, new CallContext(dataManager, 1));
        var defaultFields = ReadOnlyProviders.FirstNotNull(x => x.GetItemFields(itemDefinition, versionUri));

        if (actualFields?.Count != defaultFields?.Count)
        {
          return false;
        }

        foreach (var i in actualFields.FieldValues.Keys.Cast<ID>())
        {
          if (
            !defaultFields.FieldValues.Contains(i) ||
            actualFields[i] != defaultFields[i] ||
            false)
          {
            return false;
          }
        }

        foreach (var i in defaultFields.FieldValues.Keys.Cast<ID>())
        {
          if (
            !actualFields.FieldValues.Contains(i) ||
            actualFields[i] != defaultFields[i] ||
            false)
          {
            return false;
          }
        }
      }

      return true;
    }
  }
}
