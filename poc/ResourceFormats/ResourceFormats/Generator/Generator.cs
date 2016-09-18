using System;
using System.Collections.Generic;
using System.IO;

using ResourceFormats.Model;

namespace ResourceFormats.Generator
{
  public partial class Generator
  {
    private Stream targetStream;

    private readonly List<IReadOnlyItem> items = new List<IReadOnlyItem>();

    private readonly List<IEnumerable<IReadOnlyItem>> sources = new List<IEnumerable<IReadOnlyItem>>();

    public Generator(Stream stream)
    {
    }

    public void AddSource(IEnumerable<IReadOnlyItem> source)
    {
      this.sources.Add(source);
    }

    public void Add(IReadOnlyItem item)
    {
      this.items.Add(item);
    }

    public void Emit()
    {
      
    }
  }

  public interface IItemBuilder
  {
    IItemBuilder SetParent(Guid parentID);

    IItemBuilder SetTemplate(Guid templateID);

    IItemBuilder SetBranch(Guid branchID);   
  }


}