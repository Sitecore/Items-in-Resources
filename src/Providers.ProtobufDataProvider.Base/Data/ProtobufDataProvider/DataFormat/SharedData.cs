namespace Sitecore.Data.ProtobufDataProvider.DataFormat
{
  using System;
  using System.Collections.Generic;

  public class ItemsSharedData : Dictionary<Guid, Dictionary<Guid, string>>
  {                   
    public Dictionary<Guid, string> TryGetValue(Guid itemId)
    {
      Dictionary<Guid, string> fields;
      return !TryGetValue(itemId, out fields) ? null : fields;
    }
  }  
}