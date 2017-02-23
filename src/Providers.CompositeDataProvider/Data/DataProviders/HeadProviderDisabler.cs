namespace Sitecore.Data.DataProviders
{
  using Sitecore.Common;
  public class HeadProviderDisabler : Switcher<HeadProviderState>
  {
    protected HeadProviderDisabler() : base(HeadProviderState.Disabled)
    {                                                 
    }
  }
}
