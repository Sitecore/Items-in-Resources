using System;

namespace ResourceFormats.Generator
{
  public partial class Generator
  {
    private class ItemBuilder : IItemBuilder
    {
      private Generator.ItemInformation item;

      private Generator generator;

      public ItemBuilder(Generator generator, Generator.ItemInformation information)
      {
        this.generator = generator;
        this.item = information;
      }

      public IItemBuilder SetParent(Guid parentID)
      {
        throw new NotImplementedException();
      }

      public IItemBuilder SetTemplate(Guid templateID)
      {
        throw new NotImplementedException();
      }

      public IItemBuilder SetBranch(Guid branchID)
      {
        throw new NotImplementedException();
      }
    }
  }
}