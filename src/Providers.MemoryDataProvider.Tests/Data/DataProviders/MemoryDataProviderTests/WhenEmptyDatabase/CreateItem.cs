namespace Sitecore.Data.DataProviders.MemoryDataProviderTests.WhenEmptyDatabase
{
  public class CreateItem : AbstractDataProviderTests.WhenEmptyDatabase.CreateItem
  {
    public CreateItem()
      : base(new MemoryProviderTraits())
    {
    }
  }
}