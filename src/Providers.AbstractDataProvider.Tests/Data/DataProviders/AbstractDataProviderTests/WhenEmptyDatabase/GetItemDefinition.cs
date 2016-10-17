namespace Sitecore.Data.DataProviders.AbstractDataProviderTests.WhenEmptyDatabase
{
  using NUnit.Framework;

  [TestFixture]
    public abstract class GetItemDefinition : BaseProviderTest
    {
        protected GetItemDefinition(ProviderTraits traits)
            : base(traits)
        {
        }

        [Test]
        public void ReturnsNullWithoutException()
        {
            var provider = this.Traits.CreateProvider(this);

            var result = provider.GetItemDefinition(ID.NewID, this.Traits.CreateCallContext(this));

            Assert.That(result, Is.Null);
        }
    }
}