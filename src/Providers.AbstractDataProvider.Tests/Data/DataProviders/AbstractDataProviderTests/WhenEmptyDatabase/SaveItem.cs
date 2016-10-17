namespace Sitecore.Data.DataProviders.AbstractDataProviderTests.WhenEmptyDatabase
{
  using NUnit.Framework;

  [TestFixture]
    public abstract class SaveItem : BaseProviderTest
    {
        protected SaveItem(ProviderTraits traits)
            : base(traits)
        {
        }
    }
}