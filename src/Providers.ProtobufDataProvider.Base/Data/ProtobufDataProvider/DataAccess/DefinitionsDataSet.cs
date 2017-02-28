namespace Sitecore.Data.ProtobufDataProvider.DataAccess
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class ItemDataRecordSet : Dictionary<Guid, ItemRecord>
  {
    public ItemDataRecordSet()
    {
    }

    public ItemDataRecordSet(params ItemRecord[] items) : base(items?.ToDictionary(x => x.ID, x => x) ?? new Dictionary<Guid, ItemRecord>())
    {
    }
  }
}