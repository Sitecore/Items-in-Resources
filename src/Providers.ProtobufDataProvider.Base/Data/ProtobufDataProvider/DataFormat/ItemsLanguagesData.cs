namespace Sitecore.Data.ProtobufDataProvider.DataFormat
{
  using System;
  using System.Collections.Generic;

  public class ItemsLanguagesData : Dictionary<Guid, ItemLanguagesData>
  {            
    public new bool TryGetValue(Guid itemId, out ItemLanguagesData item)
    {
      ItemLanguagesData versions;
      if (!base.TryGetValue(itemId, out versions))
      {
        item = null;
        return false;
      }

      item = versions ?? new ItemLanguagesData();

      return true;
    }

    public ItemLanguagesData TryGetValue(Guid itemId)
    {
      ItemLanguagesData versions;
      if (!TryGetValue(itemId, out versions))
      {
        return null;
      }

      return versions;
    }
  }
}