using System;
using System.Collections.Generic;

namespace ResourceFormats.Generator
{
  public partial class Generator
  {
    private class ItemInformation
    {
      public ItemPayload Payload = new ItemPayload();

      public ItemMetadata Metadata;

      public List<ItemReference> Children;

      public ItemReference Self;

      public ItemInformation(Guid itemID)
      {
        this.Metadata = new ItemMetadata() { ID = itemID };
      }
    }
  }
}