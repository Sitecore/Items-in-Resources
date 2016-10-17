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
    public void AcceptsItemIdEmpty()
    {
      var provider = Traits.CreateProvider(this);

      provider.GetItemDefinition(ID.Null, Traits.CreateCallContext(this));
    }

    [Test]
    public void ExpectsCallContextNotNull()
    {
      var provider = Traits.CreateProvider(this);

      Assert.Throws<ArgumentNullException>(() => provider.GetItemDefinition(ID.NewID, null));
    }

    [Test]
    public void ExpectsItemIdNotNull()
    {
      var provider = Traits.CreateProvider(this);

      Assert.Throws<ArgumentNullException>(() => provider.GetItemDefinition(null, Traits.CreateCallContext(this)));
    }

    [Test]
    public void ReturnsNullWithoutException()
    {
      var provider = Traits.CreateProvider(this);

      var result = provider.GetItemDefinition(ID.NewID, Traits.CreateCallContext(this));

      Assert.That(result, Is.Null);
    }
  }
}