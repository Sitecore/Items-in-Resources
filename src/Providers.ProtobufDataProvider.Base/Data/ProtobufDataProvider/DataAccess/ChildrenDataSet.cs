namespace Sitecore.Data.ProtobufDataProvider.DataAccess
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Data.ProtobufDataProvider.DataFormat;

  public class ChildrenDataSet : Dictionary<Guid, ItemRecord[]>
  {
    public ChildrenDataSet(ItemDefinitions definitions) : base(definitions.Values.GroupBy(x => x.ParentID).ToDictionary(x => x.Key, x => x.ToArray()))
    {
    }
  }
}