namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;

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
  }
}