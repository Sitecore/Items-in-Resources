namespace Sitecore.Data.DataProviders.AbstractDataProviderTests.Contracts
{
  using System;
  using NUnit.Framework;

  [TestFixture]
    public abstract class GetItemDefinition : BaseProviderTest
    {
        protected GetItemDefinition(ProviderTraits traits)
            : base(traits)
        {
        }

        [Test]
        public void ExpectsItemIdNotNull()
        {
            var provider = this.Traits.CreateProvider(this);

            Assert.Throws<ArgumentNullException>(() => provider.GetItemDefinition(null, this.Traits.CreateCallContext(this)));
        }

        [Test]
        public void AcceptsItemIdEmpty()
        {
            var provider = this.Traits.CreateProvider(this);

            provider.GetItemDefinition(ID.Null, this.Traits.CreateCallContext(this));
        }

        [Test]
        public void ExpectsCallContextNotNull()
        {
            var provider = this.Traits.CreateProvider(this);

            Assert.Throws<ArgumentNullException>(() => provider.GetItemDefinition(ID.NewID, null));
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