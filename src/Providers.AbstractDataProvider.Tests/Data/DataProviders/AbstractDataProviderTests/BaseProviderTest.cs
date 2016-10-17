namespace Sitecore.Data.DataProviders.AbstractDataProviderTests
{
  public abstract class BaseProviderTest
    {
        private ProviderTraits traits;

        protected BaseProviderTest(ProviderTraits traits)
        {
            this.traits = traits;
        }

        protected ProviderTraits Traits
        {
            get { return this.traits; }
        }
    }
}