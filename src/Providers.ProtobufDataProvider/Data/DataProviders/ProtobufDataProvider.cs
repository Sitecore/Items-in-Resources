namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Sitecore.Collections;
  using Sitecore.Data.ProtobufDataProvider;
  using Sitecore.Data.ProtobufDataProvider.DataFormat;
  using Sitecore.Diagnostics;
  using Sitecore.Exceptions;
  using Sitecore.Extensions.Dictionary;
  using Sitecore.Globalization;

  public class ProtobufDataProvider : ReadOnlyDataProvider
  {
    private static readonly Regex DescendantsQueryRegex = new Regex(@"^fast\://\*\[([^\]]+)\]$", RegexOptions.Compiled);

    private ItemsDataSet DataSet { get; }

    [UsedImplicitly]
    public ProtobufDataProvider([NotNull] string databaseName, [NotNull] string filePath)
    {
      Assert.ArgumentNotNullOrEmpty(databaseName, nameof(databaseName));
      Assert.ArgumentNotNullOrEmpty(filePath, nameof(filePath));               

      DataSet = new ItemsDataSet(new FileInfo(MainUtil.MapPath(filePath)));
    }

    public override ItemDefinition GetItemDefinition(ID itemId)
    {
      Assert.ArgumentNotNull(itemId, nameof(itemId));

      ItemRecord item = DataSet.Definitions.TryGetValue(itemId.Guid);
      if (item == null)
      {
        return null;
      }
                           
      return new ItemDefinition(itemId, item.Name, ID.Parse(item.TemplateID), ID.Null);
    }

    public override IEnumerable<ID> GetChildIdsByName(string childName, ID parentId)
    {
      return DataSet.Children
        .TryGetValue(parentId.Guid)?
        .Where(x => x.Name.Equals(childName, StringComparison.OrdinalIgnoreCase))
        .Select(x => ID.Parse(x.ID));
    }

    public override IEnumerable<Guid> GetChildIDs(ItemDefinition itemDefinition)
    {
      // TODO: change signature to create IDList once per all data providers

      var array = DataSet.Children.TryGetValue(itemDefinition.ID.Guid);
      if (array == null)
      {
        return null;
      }
       
      return array.Select(x => x.ID);
    }

    public override ID GetParentID(ItemDefinition itemDefinition)
    {
      var item = DataSet.Definitions.TryGetValue(itemDefinition.ID.Guid);
      if (item == null)
      {
        return null;
      }

      return ID.Parse(item.ParentID);
    }

    public override VersionUriList GetItemVersions(ItemDefinition itemDefinition)
    {
      // TODO: change signature to create FieldList once per all data providers
      var list = new VersionUriList();

      var item = DataSet.LanguageData.TryGetValue(itemDefinition.ID.Guid);
      if (item == null)
      {
        return list;
      }

      foreach (var lang in item.Keys)
      {
        // in read-only data provider only single "1" version per language is supported
        list.Add(Language.Parse(lang), Data.Version.Parse(1));
      }
                        
      return list;
    }

    public override FieldList GetItemFields(ItemDefinition itemDefinition, VersionUri versionUri)
    {
      // TODO: change signature to create FieldList once per all data providers
      if (!DataSet.Definitions.ContainsKey(itemDefinition.ID.Guid))
      {
        return null;
      }

      var list = new FieldList();
      DataSet.SharedData.TryGetValue(itemDefinition.ID.Guid)?
        .ToList()
        .ForEach(sharedField =>
            list.Add(ID.Parse(sharedField.Key), sharedField.Value ?? string.Empty));

      var languages = DataSet.LanguageData.TryGetValue(itemDefinition.ID.Guid);
      if (languages != null)
      {
        foreach (var pair in languages)
        {
          if (!pair.Key.Equals(versionUri.Language.Name, StringComparison.OrdinalIgnoreCase))
          {
            continue;
          }
                                   
          // unversioned first
          pair.Value.TryGetValue(0)?
            .ToList()
            .ForEach(languageField =>
                list.Add(ID.Parse(languageField.Key), languageField.Value ?? string.Empty));

          pair.Value.TryGetValue(versionUri.Version.Number)?
            .ToList()
            .ForEach(languageField =>
                list.Add(ID.Parse(languageField.Key), languageField.Value ?? string.Empty));

          break;
        }
      }
                          
      return list;
    }

    public override IEnumerable<Guid> SelectIDs(string query)
    {
      // 18860 18:03:22 INFO  SelectSingleID: fast://*[@@templateid = '{F68F13A6-3395-426A-B9A1-FA2DC60D94EB}' and @@name = 'da']

      var desc = DescendantsQueryRegex.Match(query);
      IEnumerable<Guid> ids = null;
      if (desc.Success)
      {
        var conditionsQueryGroup = desc.Groups[1];
        var items = (IEnumerable<ItemRecord>)DataSet.Definitions.Values;
        if (conditionsQueryGroup.Success)
        {
          var conditionsQuery = conditionsQueryGroup.Value;
          if (string.IsNullOrEmpty(conditionsQuery))
          {
            throw new QueryException("Query must not contain empty square bracets []");
          }

          var conditions = conditionsQuery.Split(" and ");
          foreach (var condition in conditions)
          {
            var arr = condition.Split('=');
            Assert.IsTrue(arr.Length == 2, "wrong query");

            var keyword = arr.First().Trim();
            var value = arr.Last().Trim(" '\"".ToCharArray());
            switch (keyword)
            {
              case "@@templateid":
                var templateId = Guid.Parse(value);
                items = items.Where(x => x.TemplateID == templateId);
                break;

              case "@@name":
                var name = value;
                items = items.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                break;

              default:
                throw new NotImplementedException();
            }
          }
        }

        ids = items.Take(100).Select(x => x.ID);
      }

#if DEBUG
      Log.Info($"SelectID: {query}, Values: {string.Join(", ", ids ?? new Guid[0])}", this);
#endif

      return ids;
    }

    [NotNull]
    public override IEnumerable<Guid> GetTemplateItemIds()
    {
      var templateId = TemplateIDs.Template.Guid;
      var templates = DataSet.Definitions.Values
        .Where(x => x.TemplateID == templateId)
        .Select(x => x.ID);

      Assert.IsNotNull(templates, nameof(templates));

      return templates;
    }

    [NotNull]
    public override IEnumerable<Language> GetLanguages()
    {
      var languageList = DataSet.Children.TryGetValue(ItemIDs.LanguagesRootId);
      if (languageList == null)
      {
        return new Language[0];
      }

      var languages = languageList?
        .Select(x => x?.Name)
        .Where(x => !string.IsNullOrEmpty(x))
        .Select(x => Language.Parse(x));

      Assert.IsNotNull(languages, nameof(languages));

      return languages;
    }

    public override long GetDictionaryEntryCount()
    {
      var dictionaryEntryTemplateId = TemplateIDs.DictionaryEntry.Guid;

      return DataSet.Definitions.Values.Count(x => x.TemplateID == dictionaryEntryTemplateId);
    }
  }
}