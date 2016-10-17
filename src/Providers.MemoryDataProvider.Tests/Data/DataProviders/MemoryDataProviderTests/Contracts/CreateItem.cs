
namespace Sitecore.Data.DataProviders.MemoryDataProviderTests.Contracts
{
    public class CreateItem : Sitecore.Data.DataProviders.AbstractDataProviderTests.Contracts.CreateItem
    {
        public CreateItem()
            : base(new MemoryProviderTraits())
        {
            
        }
    }
}