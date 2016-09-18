using System;
using System.Collections.Generic;

namespace ResourceFormats
{
  [Serializable]
  internal class ItemPayload
  {
    public ItemFields SharedFields = new ItemFields();
    public Dictionary<string, ItemFields> UnversionedFields = new Dictionary<string, ItemFields>();
    public Dictionary<string, LanguageVersions> VersionedFields = new Dictionary<string, LanguageVersions>();
  }
}