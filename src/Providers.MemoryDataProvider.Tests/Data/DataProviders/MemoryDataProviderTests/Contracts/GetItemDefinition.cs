namespace Sitecore.Data.DataProviders.MemoryDataProviderTests.Contracts
{
  public class GetItemDefinition : AbstractDataProviderTests.Contracts.GetItemDefinition
  {
    public GetItemDefinition()
      : base(new MemoryProviderTraits())
    {
    }
  }
}