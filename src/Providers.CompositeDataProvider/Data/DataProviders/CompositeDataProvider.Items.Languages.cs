namespace Sitecore.Data.DataProviders
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Globalization;

  public partial class CompositeDataProvider
  {
    /* Items.Languages part of DataProvider */

    [NotNull]
    private readonly IEnumerable<Language> EmptyLanguages = new Language[0];

    public override LanguageCollection GetLanguages(CallContext context)
    {
      var languages = ReadOnlyProviders.SelectMany(x => x.GetLanguages() ?? EmptyLanguages);
      var headLanguages = HeadProvider.GetLanguages(context);
      if (headLanguages != null)
      {
        languages = languages.Concat(headLanguages);
      }
      
      languages = languages      
        .GroupBy(x => x.Name).Select(x => x.First()); // .Distinct()
        
      return new LanguageCollection(languages);
    }

    public override void RemoveLanguageData(Language language, CallContext context)
    {
      HeadProvider.RemoveLanguageData(language, context);
    }

    public override void RenameLanguageData(string fromLanguage, string toLanguage, CallContext context)
    {
      HeadProvider.RenameLanguageData(fromLanguage, toLanguage, context);
    }
  }
}