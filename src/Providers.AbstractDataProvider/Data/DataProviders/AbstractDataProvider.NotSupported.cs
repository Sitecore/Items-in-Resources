namespace Sitecore.Data.DataProviders
{
  using System;

  public abstract partial class AbstractDataProvider
  {
    protected const string MustNotBeCalled = "This method must not be called directly";

    [Obsolete(MustNotBeCalled)]
    public sealed override CacheOptions CacheOptions => base.CacheOptions;

    [Obsolete(MustNotBeCalled)]
    protected sealed override string Disable
    {
      set
      {
        base.Disable = value;
      }
    }

    [Obsolete(MustNotBeCalled)]
    protected sealed override string DisableGroup
    {
      set
      {
        base.DisableGroup = value;
      }
    }

    [Obsolete(MustNotBeCalled)]
    protected sealed override string Enable
    {
      set
      {
        base.Enable = value;
      }
    }

    [Obsolete(MustNotBeCalled)]
    protected sealed override string EnableGroup
    {
      set
      {
        base.EnableGroup = value;
      }
    }
  }
}
