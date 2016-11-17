namespace Sitecore.Data.DataProviders
{
  using System.Diagnostics;
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Extensions.Object;
  using Sitecore.Globalization;

  public partial class CompositeDataProvider
  {
    /* Items.ItemData part of DataProvider */

    public override FieldList GetItemFields(ItemDefinition itemDefinition, VersionUri versionUri, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var itemFields = HeadProvider.GetItemFields(itemDefinition, versionUri, context) 
        ?? ReadOnlyProviders.FirstNotNull(x => x.GetItemFields(itemDefinition, versionUri));

      // do not print fields
      this.Trace(null, timer, itemDefinition, versionUri, context);

      return itemFields;
    }

    public override VersionUriList GetItemVersions(ItemDefinition itemDefinition, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif
      var headList = HeadProvider.GetItemVersions(itemDefinition, context);

      if (headList != null && headList.Count > 0) return headList;

      var itemVersions = ReadOnlyProviders.Select(provider => provider.GetItemVersions(itemDefinition))
        .FirstOrDefault(list => list != null && list.Count > 0);

      this.Trace(itemVersions, timer, itemDefinition, context);

      return
        itemVersions ?? new VersionUriList();
    }

    public override int AddVersion(ItemDefinition itemDefinition, VersionUri baseVersion, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var version = HeadProvider.AddVersion(itemDefinition, baseVersion, context);

      this.Trace(version, timer, itemDefinition, baseVersion, context);

      return version;
    }

    public override bool RemoveVersion(ItemDefinition itemDefinition, VersionUri version, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var removed = HeadProvider.RemoveVersion(itemDefinition, version, context);

      this.Trace(removed, timer, itemDefinition, version, context);

      return removed;
    }

    public override bool RemoveVersions(ItemDefinition itemDefinition, Language language, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var removed = HeadProvider.RemoveVersions(itemDefinition, language, context);

      this.Trace(removed, timer, itemDefinition, language, context);

      return removed;
    }

    public override bool RemoveVersions(ItemDefinition itemDefinition, Language language, bool removeSharedData, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var removed = HeadProvider.RemoveVersions(itemDefinition, language, removeSharedData, context);

      this.Trace(removed, timer, itemDefinition, language, context);

      return removed;
    }
  }
}