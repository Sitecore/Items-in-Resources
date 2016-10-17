
namespace Sitecore.Data.DataProviders.MemoryDataProviderTests.WhenEmptyDatabase
{
    public class CreateItem : Sitecore.Data.DataProviders.AbstractDataProviderTests.WhenEmptyDatabase.CreateItem
    {
        public CreateItem()
            : base(new MemoryProviderTraits())
        {
        }
    }
}