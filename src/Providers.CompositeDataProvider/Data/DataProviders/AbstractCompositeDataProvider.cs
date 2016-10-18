namespace Sitecore.Data.DataProviders
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Extensions.DataProvider;

  public abstract class AbstractCompositeDataProvider : AbstractDataProvider
  {              
    [NotNull]
    private readonly List<ReadOnlyDataProvider> _ReadOnlyProviders = new List<ReadOnlyDataProvider>();

    [CanBeNull]
    private DataProvider _HeadProvider;

    protected AbstractCompositeDataProvider(string databaseName)
    {                                                        
      DatabaseName = databaseName;          
    }

    protected AbstractCompositeDataProvider(string databaseName, [NotNull] params DataProvider[] providers)
    {
      Assert.ArgumentNotNull(providers, nameof(providers));

      DatabaseName = databaseName;
      _ReadOnlyProviders.AddRange(providers.OfType<ReadOnlyDataProvider>());
      _HeadProvider = providers.Single(x => !(x is ReadOnlyDataProvider));
    }

    [NotNull]
    protected DataProvider HeadProvider
    {
      get
      {
        var headProvider = _HeadProvider;
        Assert.IsNotNull(headProvider, "No head provider available");

        return headProvider;
      }
    }

    protected string DatabaseName { get; }

    [NotNull]
    protected IReadOnlyCollection<ReadOnlyDataProvider> ReadOnlyProviders => _ReadOnlyProviders;

    [UsedImplicitly]
    public void AddDataProvider([NotNull] DataProvider dataProvider)
    {
      Assert.ArgumentNotNull(dataProvider, nameof(dataProvider));

      lock (_ReadOnlyProviders)
      {
        var readOnlyDataProvider = dataProvider as ReadOnlyDataProvider;
        if (readOnlyDataProvider == null)
        {
          Assert.IsNull(_HeadProvider, "The head provider is already defined");

          Log.Info($"Add [{DatabaseName}] HEAD data provider: {dataProvider.GetType().FullName}", this);

          _HeadProvider = dataProvider;
        }
        else
        {
          Log.Info($"Add [{DatabaseName}] read-only data provider: {dataProvider.GetType().FullName}", this);

          _ReadOnlyProviders.Add(readOnlyDataProvider);
        }
      }
    }

    protected override void Initialize()
    {
      base.Initialize();

      HeadProvider.SetDatabase(Database);

      foreach (var provider in ReadOnlyProviders)
      {
        provider?.SetDatabase(Database);
      }
    }
  }
}