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
      var provider = Traits.CreateProvider(this);

      Assert.Throws<ArgumentNullException>(
        () =>
          provider.CreateItem(
            null,
            ID.NewID.ToString(),
            ID.NewID,
            Traits.CreateFakeDefinition(this),
            Traits.CreateCallContext(this)));
    }

    [Test]
    public void ExpectsItemIdNotEmpty()
    {
      var provider = Traits.CreateProvider(this);

      Assert.Throws<ArgumentException>(
        () =>
          provider.CreateItem(
            ID.Null,
            ID.NewID.ToString(),
            ID.NewID,
            Traits.CreateFakeDefinition(this),
            Traits.CreateCallContext(this)));
    }

    [Test]
    public void ExpectsNameNotNull()
    {
      var provider = Traits.CreateProvider(this);

      Assert.Throws<ArgumentNullException>(
        () =>
          provider.CreateItem(
            ID.NewID,
            null,
            ID.NewID,
            Traits.CreateFakeDefinition(this),
            Traits.CreateCallContext(this)));
    }

    [Test]
    public void ExpectsNameNotEmpty()
    {
      var provider = Traits.CreateProvider(this);

      Assert.Throws<ArgumentException>(
        () =>
          provider.CreateItem(
            ID.NewID,
            string.Empty,
            ID.NewID,
            Traits.CreateFakeDefinition(this),
            Traits.CreateCallContext(this)));
    }

    [Test]
    public void ExpectsTemplateIdNotNull()
    {
      var provider = Traits.CreateProvider(this);

      Assert.Throws<ArgumentNullException>(
        () =>
          provider.CreateItem(
            ID.NewID,
            ID.NewID.ToString(),
            null,
            Traits.CreateFakeDefinition(this),
            Traits.CreateCallContext(this)));
    }

    [Test]
    public void ExpectsTemplateIdNotEmpty()
    {
      var provider = Traits.CreateProvider(this);

      Assert.Throws<ArgumentException>(
        () =>
          provider.CreateItem(
            ID.NewID,
            ID.NewID.ToString(),
            ID.Null,
            Traits.CreateFakeDefinition(this),
            Traits.CreateCallContext(this)));
    }

    [Test]
    public void ExpectsParentNotNull()
    {
      var provider = Traits.CreateProvider(this);

      Assert.Throws<ArgumentNullException>(
        () =>
          provider.CreateItem(
            ID.NewID,
            ID.NewID.ToString(),
            ID.NewID,
            null,
            Traits.CreateCallContext(this)));
    }

    [Test]
    public void ExpectsContextNotNull()
    {
      var provider = Traits.CreateProvider(this);

      Assert.Throws<ArgumentNullException>(
        () =>
          provider.CreateItem(
            ID.NewID,
            ID.NewID.ToString(),
            ID.NewID,
            Traits.CreateFakeDefinition(this),
            null));
    }
  }
}