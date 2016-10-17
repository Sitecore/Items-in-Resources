namespace Sitecore.Data.DataProviders
{
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Extensions;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Globalization;

  public partial class CompositeDataProvider
  {
    /* Items.ItemData part of DataProvider */

    public override FieldList GetItemFields(ItemDefinition itemDefinition, VersionUri versionUri, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetItemFields(itemDefinition, versionUri, context));
    }          

    public override VersionUriList GetItemVersions(ItemDefinition itemDefinition, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetItemVersions(itemDefinition, context));
    }

    public override bool SaveItem(ItemDefinition itemDefinition, ItemChanges changes, CallContext context)
    {
      return Providers.Any(x => x.SaveItem(itemDefinition, changes, context));
    }

    public override int AddVersion(ItemDefinition itemDefinition, VersionUri baseVersion, CallContext context)
    {
      return Providers.FirstPositive(x => x.AddVersion(itemDefinition, baseVersion, context), -1);
    }

    public override bool RemoveVersion(ItemDefinition itemDefinition, VersionUri version, CallContext context)
    {
      return Providers.Any(x => x.RemoveVersion(itemDefinition, version, context));
    }

    public override bool RemoveVersions(ItemDefinition itemDefinition, Language language, CallContext context)
    {
      return Providers.Any(x => x.RemoveVersions(itemDefinition, language, context));
    }

    public override bool RemoveVersions(ItemDefinition itemDefinition, Language language, bool removeSharedData, CallContext context)
    {
      return Providers.Any(x => x.RemoveVersions(itemDefinition, language, removeSharedData, context));
    }
  }
}