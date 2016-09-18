using System;

namespace ResourceFormats
{
  internal class ItemReference
  {
    public Guid ID;

    public long MetadataOffset;

    public long PayloadOffset;
  }
}