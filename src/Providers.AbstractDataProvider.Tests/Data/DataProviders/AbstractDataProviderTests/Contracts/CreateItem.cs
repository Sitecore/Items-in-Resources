namespace Sitecore.Data.DataProviders.AbstractDataProviderTests.Contracts
{
  using System;
  using NUnit.Framework;

  [TestFixture]
    public abstract class CreateItem : BaseProviderTest
    {
        protected CreateItem(ProviderTraits traits)
            : base(traits)
        {
        }

        [Test]
        public void ExpectsItemIdNotNull()
        {
            var provider = this.Traits.CreateProvider(this);

            Assert.Throws<ArgumentNullException>(
                () =>
                    provider.CreateItem(
                        null,
                        ID.NewID.ToString(),
                        ID.NewID,
                        this.Traits.CreateFakeDefinition(this),
                        this.Traits.CreateCallContext(this)));
        }

        [Test]
        public void ExpectsItemIdNotEmpty()
        {
            var provider = this.Traits.CreateProvider(this);

            Assert.Throws<ArgumentException>(
                () =>
                    provider.CreateItem(
                        ID.Null,
                        ID.NewID.ToString(),
                        ID.NewID,
                        this.Traits.CreateFakeDefinition(this),
                        this.Traits.CreateCallContext(this)));
        }

        [Test]
        public void ExpectsNameNotNull()
        {
            var provider = this.Traits.CreateProvider(this);

            Assert.Throws<ArgumentNullException>(
                () =>
                    provider.CreateItem(
                        ID.NewID,
                        null,
                        ID.NewID,
                        this.Traits.CreateFakeDefinition(this),
                        this.Traits.CreateCallContext(this)));
        }

        [Test]
        public void ExpectsNameNotEmpty()
        {
            var provider = this.Traits.CreateProvider(this);

            Assert.Throws<ArgumentException>(
                () =>
                    provider.CreateItem(
                        ID.NewID,
                        string.Empty,
                        ID.NewID,
                        this.Traits.CreateFakeDefinition(this),
                        this.Traits.CreateCallContext(this)));
        }

        [Test]
        public void ExpectsTemplateIdNotNull()
        {
            var provider = this.Traits.CreateProvider(this);

            Assert.Throws<ArgumentNullException>(
                () =>
                    provider.CreateItem(
                        ID.NewID,
                        ID.NewID.ToString(),
                        null,
                        this.Traits.CreateFakeDefinition(this),
                        this.Traits.CreateCallContext(this)));
        }

        [Test]
        public void ExpectsTemplateIdNotEmpty()
        {
            var provider = this.Traits.CreateProvider(this);

            Assert.Throws<ArgumentException>(
                () =>
                    provider.CreateItem(
                        ID.NewID,
                        ID.NewID.ToString(),
                        ID.Null,
                        this.Traits.CreateFakeDefinition(this),
                        this.Traits.CreateCallContext(this)));
        }

        [Test]
        public void ExpectsParentNotNull()
        {
            var provider = this.Traits.CreateProvider(this);

            Assert.Throws<ArgumentNullException>(
                () =>
                    provider.CreateItem(
                        ID.NewID,
                        ID.NewID.ToString(),
                        ID.NewID,
                        null,
                        this.Traits.CreateCallContext(this)));
        }

        [Test]
        public void ExpectsContextNotNull()
        {
            var provider = this.Traits.CreateProvider(this);

            Assert.Throws<ArgumentNullException>(
                () =>
                    provider.CreateItem(
                        ID.NewID,
                        ID.NewID.ToString(),
                        ID.NewID,
                        this.Traits.CreateFakeDefinition(this),
                        null));
        }
    }
}