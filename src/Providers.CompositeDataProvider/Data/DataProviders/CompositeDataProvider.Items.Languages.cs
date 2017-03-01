namespace Sitecore.Data.DataProviders
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Extensions.Object;
  using Sitecore.Globalization;

  public partial class CompositeDataProvider
  {
    /* Items.Languages part of DataProvider */

    [NotNull]
    private readonly IEnumerable<Language> EmptyLanguages = new Language[0];

    public override LanguageCollection GetLanguages(CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var languages = ReadOnlyProviders.SelectMany(x => x.GetLanguages() ?? EmptyLanguages);
      var headLanguages = HeadProvider.GetLanguages(context);
      if (headLanguages != null)
      {
        languages = languages.Concat(headLanguages);
      }
      
      languages = languages      
        .GroupBy(x => x.Name).Select(x => x.First()); // .Distinct()
        

#if DEBUG
      this.Trace(languages, timer, context);
#endif

      return new LanguageCollection(languages);
    }

    public override void RemoveLanguageData(Language language, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      HeadProvider.RemoveLanguageData(language, context);

#if DEBUG
      this.Trace(null, timer, language, context);
#endif
    }

    public override void RenameLanguageData(string fromLanguage, string toLanguage, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      HeadProvider.RenameLanguageData(fromLanguage, toLanguage, context);

#if DEBUG
      this.Trace(null, timer, fromLanguage, toLanguage, context);
#endif
    }
  }
}