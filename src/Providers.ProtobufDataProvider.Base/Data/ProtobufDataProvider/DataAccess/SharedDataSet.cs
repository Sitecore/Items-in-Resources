namespace Sitecore.Data.ProtobufDataProvider.DataAccess
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
        SharedFields = Serializer.Deserialize<Dictionary<Guid, Dictionary<Guid, string>>>(reader);
      }
    }

    public Dictionary<Guid, string> TryGetValue(Guid itemId)
    {
      Dictionary<Guid, string> fields;
      return !SharedFields.TryGetValue(itemId, out fields) ? null : fields;
    }
  }
}