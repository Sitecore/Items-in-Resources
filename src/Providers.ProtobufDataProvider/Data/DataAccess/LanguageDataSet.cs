namespace Sitecore.Data.DataAccess
{
  using System;
  using System.IO;
  using Sitecore.Data.DataFormat;
  using ProtoBuf;

  public class LanguageDataSet 
  {
    public readonly ItemsLanguagesData ItemsLanguageFields;

    public LanguageDataSet(Stream reader)
    {
      using (reader)
      {
        ItemsLanguageFields = Serializer.Deserialize<ItemsLanguagesData>(reader);
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

      item = versions ?? new ItemLanguagesData();

      return true;
    }

    public ItemLanguagesData TryGetValue(Guid itemId)
    {
      ItemLanguagesData versions;
      if (!ItemsLanguageFields.TryGetValue(itemId, out versions))
      {
        return null;
      }

      return versions;
    }
  }
}