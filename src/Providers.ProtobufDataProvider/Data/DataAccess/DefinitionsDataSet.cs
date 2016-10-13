namespace Sitecore.Data.DataAccess
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class ItemInfoSet : Dictionary<Guid, ItemInfo>
  {
    public ItemInfoSet()
    {
    }
                              
    public ItemInfoSet(params ItemInfo[] items) : base(items?.ToDictionary(x => x.ID, x => x) ?? new Dictionary<Guid, ItemInfo>())
    { 
    }
  }
}