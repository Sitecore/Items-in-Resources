namespace Sitecore.Data.DataProviders.MemoryDataProviderTests
{
  using Sitecore.Data.DataProviders.AbstractDataProviderTests;

  public class MemoryProviderTraits : ProviderTraits
  {
    public override DataProvider CreateProvider(BaseProviderTest test)
    {
      return new MemoryDataProvider();
    }
  }
}