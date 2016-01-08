namespace IR.Data.DataAccess
{
  using System;
  using System.IO;
  using IR.Data.DataFormat;
  using ProtoBuf;

  public class LanguageDataSet 
  {
    public readonly ItemsLanguagesData ItemsLanguageFields;

    public LanguageDataSet(Stream reader)
    {
      using (reader)
      {
        this.ItemsLanguageFields = Serializer.Deserialize<ItemsLanguagesData>(reader);
      }
    }

    public bool TryGetValue(Guid itemId, out ItemLanguagesData item)
    {
      ItemLanguagesData versions;
      if (!ItemsLanguageFields.TryGetValue(itemId, out versions))
      {
        item = null;
        return false;
      }

      item = versions ?? Empty.Versions;
      return true;
    }
  }
}