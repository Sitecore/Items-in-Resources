
namespace Sitecore.Data.DataProviders.MemoryDataProviderTests.WhenEmptyDatabase
{
    public class SaveItem : Sitecore.Data.DataProviders.AbstractDataProviderTests.WhenEmptyDatabase.SaveItem
    {
        public SaveItem()
            : base(new MemoryProviderTraits())
        {
        }
    }
}