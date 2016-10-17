namespace Sitecore.Data.DataProviders.MemoryDataProviderTests.Contracts
{
  public class CreateItem : AbstractDataProviderTests.Contracts.CreateItem
  {
    public CreateItem()
      : base(new MemoryProviderTraits())
    {
    }
  }
}