namespace IR.Data.DataAccess
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using ProtoBuf;

  public class SharedDataSet 
  {
    public readonly Dictionary<Guid, Dictionary<Guid, string>> SharedFields;       

    public SharedDataSet(Stream reader)
    {
      using (reader)
      {
        this.SharedFields = Serializer.Deserialize<Dictionary<Guid, Dictionary<Guid, string>>>(reader);
      }                               
    }

    public bool TryGetValue(Guid itemId, out Dictionary<Guid, string> sharedFields)
    {
      Dictionary<Guid, string> fields;
      if (!SharedFields.TryGetValue(itemId, out fields))
      {
        sharedFields = null;
        return false;
      }

      sharedFields = fields ?? Empty.Fields;
        
      return true;
    }
  }
}