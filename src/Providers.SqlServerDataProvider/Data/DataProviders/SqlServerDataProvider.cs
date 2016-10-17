namespace Sitecore.Data.DataProviders
{
  using System;

  public sealed class SqlServerDataProvider : Data.SqlServer.SqlServerDataProvider
  {
    private readonly ItemDefinition RootItemDefinition = new ItemDefinition(ItemIDs.RootID, "sitecore", ID.Parse("{C6576836-910C-4A3D-BA03-C277DBD3B827}"), new ID(Guid.Empty), DateTime.Parse("2016-05-19 08:41:28.000"));

    [UsedImplicitly]
    public SqlServerDataProvider(string connectionString) : base(connectionString)
    {
    }

    public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
    {
      // FIX for NullReference exception in ResolvePathRec
      if (itemId == ItemIDs.RootID)
      {
        return RootItemDefinition;
      }

      return base.GetItemDefinition(itemId, context);
    }
  }
}