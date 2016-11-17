namespace Sitecore.Data.DataProviders
{
  using System;
  using Sitecore.Eventing;

  public abstract partial class AbstractDataProvider : DataProvider
  {
    private bool _DatabaseInitialized;

    [Obsolete(MustNotBeCalled)]
    public sealed override EventQueue GetEventQueue()
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
                                 
    /// <summary>
    /// Must not be called directly.
    /// </summary>
    protected virtual void Initialize()
    { 
    }

    /// <summary>
    /// Must not be called directly.
    /// </summary>
    protected virtual EventQueue DoGetEventQueue()
    {
      return null;
    }
  }
}
