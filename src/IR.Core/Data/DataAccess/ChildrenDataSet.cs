namespace IR.Data.DataAccess
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class ChildrenDataSet : Dictionary<Guid, ItemInfo[]>
  {                                                          
    public ChildrenDataSet(ItemInfoSet definitions) : base(definitions.Values.GroupBy(x => x.ParentID).ToDictionary(x => x.Key, x => x.ToArray()))
    {         
    }
  }
}