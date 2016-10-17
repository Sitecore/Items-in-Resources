namespace Sitecore.Data.DataProviders.MemoryDataProviderTests.WhenEmptyDatabase
{
  public class SaveItem : AbstractDataProviderTests.WhenEmptyDatabase.SaveItem
  {
    public SaveItem()
      : base(new MemoryProviderTraits())
    {
    }
  }
}