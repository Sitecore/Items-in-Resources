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

      var itemFields = HeadProvider?.GetItemFields(itemDefinition, versionUri, context) 
        ?? ReadOnlyProviders.FirstNotNull(x => x.GetItemFields(itemDefinition, versionUri));

      // do not print fields
#if DEBUG
      this.Trace(null, timer, itemDefinition, versionUri, context);
#endif

      return itemFields;
    }

    public override VersionUriList GetItemVersions(ItemDefinition itemDefinition, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif
      var headList = HeadProvider?.GetItemVersions(itemDefinition, context);

      if (headList != null && headList.Count > 0) return headList;

      var itemVersions = ReadOnlyProviders.Select(provider => provider.GetItemVersions(itemDefinition))
        .FirstOrDefault(list => list != null && list.Count > 0);

#if DEBUG
      this.Trace(itemVersions, timer, itemDefinition, context);
#endif

      return
        itemVersions ?? new VersionUriList();
    }

    public override int AddVersion(ItemDefinition itemDefinition, VersionUri baseVersion, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var version = HeadProvider?.AddVersion(itemDefinition, baseVersion, context) ?? -1;

#if DEBUG
      this.Trace(version, timer, itemDefinition, baseVersion, context);
#endif

      return version;
    }

    public override bool RemoveVersion(ItemDefinition itemDefinition, VersionUri version, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var removed = HeadProvider?.RemoveVersion(itemDefinition, version, context) ?? false;

#if DEBUG
      this.Trace(removed, timer, itemDefinition, version, context);
#endif

      return removed;
    }

    public override bool RemoveVersions(ItemDefinition itemDefinition, Language language, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var removed = HeadProvider?.RemoveVersions(itemDefinition, language, context) ?? false;

#if DEBUG
      this.Trace(removed, timer, itemDefinition, language, context);
#endif

      return removed;
    }

    public override bool RemoveVersions(ItemDefinition itemDefinition, Language language, bool removeSharedData, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var removed = HeadProvider?.RemoveVersions(itemDefinition, language, removeSharedData, context) ?? false;

#if DEBUG
      this.Trace(removed, timer, itemDefinition, language, context);
#endif

      return removed;
    }
  }
}