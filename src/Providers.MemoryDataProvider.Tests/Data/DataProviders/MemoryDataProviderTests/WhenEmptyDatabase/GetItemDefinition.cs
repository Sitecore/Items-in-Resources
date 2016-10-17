namespace Sitecore.Data.DataProviders.MemoryDataProviderTests.WhenEmptyDatabase
{
  public class GetItemDefinition : AbstractDataProviderTests.WhenEmptyDatabase.GetItemDefinition
  {
    public GetItemDefinition()
      : base(new MemoryProviderTraits())
    {
    }
  }
}