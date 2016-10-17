namespace Sitecore.Data.DataProviders
{
  using Sitecore.Collections;
  using Sitecore.Extensions;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Globalization;

  public partial class CompositeDataProvider
  {
    /* Items.Languages part of DataProvider */

    public override LanguageCollection GetLanguages(CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetLanguages(context));
    }

    public override void RemoveLanguageData(Language language, CallContext context)
    {
      Providers.ForEach(x => x.RemoveLanguageData(language, context));
    }

    public override void RenameLanguageData(string fromLanguage, string toLanguage, CallContext context)
    {
      Providers.ForEach(x => x.RenameLanguageData(fromLanguage, toLanguage, context));
    }            
  }
}