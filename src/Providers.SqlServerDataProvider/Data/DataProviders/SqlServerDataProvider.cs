namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Collections;

  public sealed class SqlServerDataProvider : Data.SqlServer.SqlServerDataProvider, IDataProviderEx
  {
    private readonly ItemDefinition RootItemDefinition = new ItemDefinition(Sitecore.ItemIDs.RootID, "sitecore", ID.Parse("{C6576836-910C-4A3D-BA03-C277DBD3B827}"), new ID(Guid.Empty), DateTime.Parse("2016-05-19 08:41:28.000"));

    [UsedImplicitly]
    public SqlServerDataProvider(string connectionString) : base(connectionString)
    {
    }

    public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
    {
      // FIX for NullReference exception in ResolvePathRec
      if (itemId == Sitecore.ItemIDs.RootID)
      {
        return RootItemDefinition;
      }

      return base.GetItemDefinition(itemId, context);
    }

    public new IEnumerable<ID> GetChildIdsByName(string childName, ID parentId)
    {
      return base.GetChildIdsByName(childName, parentId);
    }

    public override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
    {
      // to bypass prefetch cache 
      // as prefetch cache doesn't work when parent item is not in SQL database
      using (new DatabaseCacheDisabler())
      {
        return base.GetChildIDs(itemDefinition, context);
      }
    }
  }
}