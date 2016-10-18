namespace Sitecore.Data.DataProviders
{
  using System;

  public abstract partial class AbstractDataProvider
  {
    private const string MustNotBeCalledDirectly = "This method must not be called directly";

    [Obsolete(MustNotBeCalledDirectly)]
    public sealed override CacheOptions CacheOptions => base.CacheOptions;

    [Obsolete(MustNotBeCalledDirectly)]
    protected sealed override string Disable
    {
      set
      {
        base.Disable = value;
      }
    }

    [Obsolete(MustNotBeCalledDirectly)]
    protected sealed override string DisableGroup
    {
      set
      {
        base.DisableGroup = value;
      }
    }

    [Obsolete(MustNotBeCalledDirectly)]
    protected sealed override string Enable
    {
      set
      {
        base.Enable = value;
      }
    }

    [Obsolete(MustNotBeCalledDirectly)]
    protected sealed override string EnableGroup
    {
      set
      {
        base.EnableGroup = value;
      }
    }
  }
}
