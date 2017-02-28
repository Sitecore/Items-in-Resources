namespace Sitecore.Data.ProtobufDataProvider.DataAccess
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class ChildrenDataSet : Dictionary<Guid, ItemRecord[]>
  {
    public ChildrenDataSet(ItemDataRecordSet definitions) : base(definitions.Values.GroupBy(x => x.ParentID).ToDictionary(x => x.Key, x => x.ToArray()))
    {
    }
  }
}