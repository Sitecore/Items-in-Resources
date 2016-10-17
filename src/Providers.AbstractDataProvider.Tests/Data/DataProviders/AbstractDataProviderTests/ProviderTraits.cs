

namespace Sitecore.Data.DataProviders.AbstractDataProviderTests
{
  using Sitecore.Data.DataProviders;

  public abstract class ProviderTraits
    {
        public abstract DataProvider CreateProvider(BaseProviderTest test);

        public virtual CallContext CreateCallContext(BaseProviderTest test)
        {
            return new CallContext(null, 0);
        }

        public virtual ItemDefinition CreateFakeDefinition(BaseProviderTest test)
        {
            return new ItemDefinition(ID.NewID, ID.NewID.ToString(), ID.NewID, ID.NewID);
        }
    }
}