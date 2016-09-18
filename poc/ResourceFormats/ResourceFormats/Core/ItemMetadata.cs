using System;

namespace ResourceFormats
{
  internal class ItemMetadata
  {
    public Guid ID;

    public Guid ParentID;

    public Guid TemplateID;

    public Guid BranchID;

    public string Name;

    public long PayloadOffset;
  }
}