namespace Sitecore.Data.DataProviders
{
  using Sitecore.Eventing;

  public abstract partial class AbstractDataProvider : DataProvider
  {                     
    private bool _DatabaseInitialized;

    public override EventQueue GetEventQueue()
    {
      // this method is the only place where we can track initialization event

      if (!_DatabaseInitialized)
      {
        lock (this)
        {
          if (!_DatabaseInitialized)
          {            
            Initialize();

            _DatabaseInitialized = true;
          }
        }
      }

      return DoGetEventQueue();
    }

    protected virtual void Initialize()
    { 
    }

    protected virtual EventQueue DoGetEventQueue()
    {
      return null;
    }
  }
}
