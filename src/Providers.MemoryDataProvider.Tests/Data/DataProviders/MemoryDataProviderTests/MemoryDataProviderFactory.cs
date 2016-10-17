namespace Sitecore.Data.DataProviders.MemoryDataProviderTests
{
  using Sitecore.Data.DataProviders.AbstractDataProviderTests;

  public static class MemoryDataProviderFactory
  {
    public static ProviderTraits CreateTraits()
    {
      return new MemoryProviderTraits();
    }
  }
}