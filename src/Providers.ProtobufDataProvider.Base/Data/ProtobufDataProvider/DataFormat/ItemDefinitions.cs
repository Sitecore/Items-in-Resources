namespace Sitecore.Data.ProtobufDataProvider.DataFormat
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class ItemDefinitions : Dictionary<Guid, ItemRecord>
  {
    public ItemDefinitions()
    {
    }

    public ItemDefinitions(params ItemRecord[] items) : base(items?.ToDictionary(x => x.ID, x => x) ?? new Dictionary<Guid, ItemRecord>())
    {
    }
  }
}