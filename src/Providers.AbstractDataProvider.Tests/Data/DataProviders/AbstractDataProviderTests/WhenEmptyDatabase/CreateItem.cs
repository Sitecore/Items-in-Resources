﻿namespace Sitecore.Data.DataProviders.AbstractDataProviderTests.WhenEmptyDatabase
{
  using NUnit.Framework;

  [TestFixture]
    public abstract class CreateItem : BaseProviderTest
    {
        protected CreateItem(ProviderTraits traits)
            : base(traits)
        {
        }

        [Test]
        public void DoesNotThrowAndCanGetItem()
        {
            var provider = Traits.CreateProvider(this);

            var fakeParent = CreateFakeParent();
            var result = provider.CreateItem(ID.NewID, ID.NewID.ToString(), ID.NewID, fakeParent, Traits.CreateCallContext(this));

            Assert.That(result, Is.True);
        }

        [Test]
        public void PutsItemIntoDatabase()
        {
            var provider = Traits.CreateProvider(this);

            var fakeParent = new ItemDefinition(ID.NewID, ID.NewID.ToString(), ID.NewID, ID.NewID);
            var itemID = ID.NewID;
            provider.CreateItem(itemID, ID.NewID.ToString(), ID.NewID, fakeParent, Traits.CreateCallContext(this));

            var itemDefinition = provider.GetItemDefinition(itemID, Traits.CreateCallContext(this));
            Assert.That(itemDefinition, Is.Not.Null);
        }

        [Test]
        public void AddsToParentChildren()
        {
            var provider = Traits.CreateProvider(this);

            var fakeParent = new ItemDefinition(ID.NewID, ID.NewID.ToString(), ID.NewID, ID.NewID);
            var parentId = ID.NewID;
            provider.CreateItem(parentId, ID.NewID.ToString(), ID.NewID, fakeParent, Traits.CreateCallContext(this));

            var parent = provider.GetItemDefinition(parentId, Traits.CreateCallContext(this));
            Assert.That(parent, Is.Not.Null);

            var itemId = ID.NewID;
            provider.CreateItem(itemId, ID.NewID.ToString(), ID.NewID, parent, Traits.CreateCallContext(this));

            var children = provider.GetChildIDs(parent, Traits.CreateCallContext(this));

            Assert.That(children, Is.EquivalentTo(new [] { itemId }));
        }

        protected virtual ItemDefinition CreateFakeParent()
        {
            var fakeParent = new ItemDefinition(ID.NewID, ID.NewID.ToString(), ID.NewID, ID.NewID);
            return fakeParent;
        }
    }
}