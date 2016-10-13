namespace Sitecore.Data.DataProviders
{
  using Sitecore.Diagnostics;
  using Sitecore.Eventing;
  using Sitecore.Extensions;
  using Sitecore.Reflection;

  public partial class CompositeDataProvider
  {
    /* EventQueue part of DataProvider */

    private bool _DatabaseInitialized;

    public override EventQueue GetEventQueue()
    {
      if (!_DatabaseInitialized)
      {
        lock (this)
        {
          if (!_DatabaseInitialized)
          {
            foreach (var dataProvider in Providers)
            {                                                        
              Log.Info($"Initializing {Database.Name} composite data provider", this);
              ReflectionUtil.SetProperty(dataProvider, "Database", Database);
            }

            _DatabaseInitialized = true;
          }
        }
      }

      return Providers.FirstNotNull(x => x.GetEventQueue());
    }
  }
}