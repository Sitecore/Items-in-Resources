namespace Sitecore.Data.DataProviders
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Diagnostics;

  public sealed partial class CompositeDataProvider : DataProvider
  {
    [NotNull]
    private readonly List<DataProvider> _Providers;

    [UsedImplicitly]
    public CompositeDataProvider()
    {
      _Providers = new List<DataProvider>();
    }

    public CompositeDataProvider([NotNull] params DataProvider[] providers)
    {
      Assert.ArgumentNotNull(providers, nameof(providers));

      _Providers = providers.ToList();
    }

    [NotNull]
    public IReadOnlyCollection<DataProvider> Providers => _Providers;

    [UsedImplicitly]
    public void AddDataProvider([NotNull] DataProvider dataProvider)
    {
      Assert.ArgumentNotNull(dataProvider, nameof(dataProvider));

      lock (_Providers)
      {
        Log.Info("Add " + dataProvider.GetType().FullName, this);

        _Providers.Add(dataProvider);
      }
    }
  }
}