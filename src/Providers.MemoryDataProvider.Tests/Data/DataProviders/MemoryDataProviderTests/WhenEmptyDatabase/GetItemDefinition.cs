
namespace Sitecore.Data.DataProviders.MemoryDataProviderTests.WhenEmptyDatabase
{
    public class GetItemDefinition : Sitecore.Data.DataProviders.AbstractDataProviderTests.WhenEmptyDatabase.GetItemDefinition
    {
        public GetItemDefinition()
            : base(new MemoryProviderTraits())
        {
        }
    }
}