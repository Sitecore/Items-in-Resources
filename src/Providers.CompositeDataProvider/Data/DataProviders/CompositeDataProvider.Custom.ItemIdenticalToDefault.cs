namespace Sitecore.Data.DataProviders
{
  using System.Linq;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Extensions.Enumerable;

  partial class CompositeDataProvider
  {                  
    public bool CanBeRemovedFromHead([NotNull] Item item)
    {
      Assert.ArgumentNotNull(item, nameof(item));

      var dataManager = item.Database.DataManager;
      if (HeadProvider.GetItemDefinition(item.ID, new CallContext(dataManager, 1)) == null)
      {
        return false;
      }

      foreach (var version in item.Versions.GetVersions(true))
      {
        var itemDefinition = new ItemDefinition(version.ID, version.Name, version.TemplateID, version.BranchId);
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
