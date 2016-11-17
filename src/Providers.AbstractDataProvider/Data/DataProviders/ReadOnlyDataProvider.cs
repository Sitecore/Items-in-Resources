namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Collections;
  using Sitecore.Globalization;

  public abstract partial class ReadOnlyDataProvider : AbstractDataProvider, IDataProviderEx
  {                      
    [CanBeNull]
    public abstract IEnumerable<Guid> SelectIDs(string query);

    [CanBeNull]
    public abstract IEnumerable<Guid> GetTemplateItemIds();

    [CanBeNull]
    public abstract IEnumerable<Language> GetLanguages();

    [CanBeNull]
    public abstract FieldList GetItemFields(ItemDefinition itemDefinition, VersionUri versionUri);

    [CanBeNull]
    public abstract VersionUriList GetItemVersions(ItemDefinition itemDefinition);

    [CanBeNull]
    public abstract ID GetParentID(ItemDefinition itemDefinition);

    [CanBeNull]
    public abstract IEnumerable<Guid> GetChildIDs(ItemDefinition itemDefinition);

    [CanBeNull]
    public abstract ItemDefinition GetItemDefinition(ID itemId);

    public abstract IEnumerable<ID> GetChildIdsByName(string childName, ID parentId);
  }
}
