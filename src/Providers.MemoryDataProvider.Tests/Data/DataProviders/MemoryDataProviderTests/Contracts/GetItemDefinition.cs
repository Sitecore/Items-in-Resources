namespace Sitecore.Data.DataProviders.MemoryDataProviderTests.Contracts
{
  public class GetItemDefinition : Sitecore.Data.DataProviders.AbstractDataProviderTests.Contracts.GetItemDefinition
    {
        public GetItemDefinition()
            : base(new MemoryProviderTraits())
        {
        }
    }
}