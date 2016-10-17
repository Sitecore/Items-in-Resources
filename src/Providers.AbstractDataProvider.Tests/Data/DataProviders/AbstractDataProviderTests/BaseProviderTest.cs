namespace Sitecore.Data.DataProviders.AbstractDataProviderTests
{
  public abstract class BaseProviderTest
  {
    protected BaseProviderTest(ProviderTraits traits)
    {
      Traits = traits;
    }

    protected ProviderTraits Traits { get; }
  }
}