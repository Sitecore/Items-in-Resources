﻿namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Sitecore.Collections;
  using Sitecore.Data.DataAccess;
  using Sitecore.Data.DataFormat;
  using Sitecore.Diagnostics;
  using Sitecore.Exceptions;
  using Sitecore.Extensions.Dictionary;
  using Sitecore.Globalization;

  public class ProtobufDataProvider : ReadOnlyDataProvider
  {
    private static readonly Regex DescendantsQueryRegex = new Regex(@"^fast\://\*(\[[^\]])$", RegexOptions.Compiled);

    [NotNull]
    private readonly string DatabaseName;

    private readonly DataSet DataSet;

    [UsedImplicitly]
    public ProtobufDataProvider([NotNull] string databaseName, [NotNull] string definitionsFilePath, [NotNull] string sharedDataFilePath, [NotNull] string languageDataFilePath)
    {
      Assert.ArgumentNotNullOrEmpty(databaseName, nameof(databaseName));
      Assert.ArgumentNotNullOrEmpty(definitionsFilePath, nameof(definitionsFilePath));
      Assert.ArgumentNotNullOrEmpty(sharedDataFilePath, nameof(sharedDataFilePath));
      Assert.ArgumentNotNullOrEmpty(languageDataFilePath, nameof(languageDataFilePath));

      DatabaseName = databaseName;
      DataSet = new DataSet(new FileInfo(MainUtil.MapPath(definitionsFilePath)), new FileInfo(MainUtil.MapPath(sharedDataFilePath)), new FileInfo(MainUtil.MapPath(languageDataFilePath)));
    }

    public override ItemDefinition GetItemDefinition(ID itemId)
    {
      Assert.ArgumentNotNull(itemId, nameof(itemId));

      ItemInfo item = DataSet.ItemInfo.TryGetValue(itemId.Guid);
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
      var item = DataSet.ItemInfo.TryGetValue(itemDefinition.ID.Guid);
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

      if (versionUri.Version.Number > 1)
      {
        return null;
      }

      if (!DataSet.ItemInfo.ContainsKey(itemDefinition.ID.Guid))
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

          pair.Value?
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
      if (desc.NextMatch().Success)
      {
        var conditionsQueryGroup = desc.Groups[1];
        var items = (IEnumerable<ItemInfo>)DataSet.ItemInfo.Values;
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
            var value = arr.Last().Trim();
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
      var templates = DataSet.ItemInfo.Values
        .Where(x => x.TemplateID == templateId)
        .Select(x => x.ID);

      Assert.IsNotNull(templates, nameof(templates));

      return templates;
    }

    [NotNull]
    public override IEnumerable<Language> GetLanguages()
    {
      var languageList = this.DataSet.Children.TryGetValue(ItemIDs.LanguagesRootId);
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

      return DataSet.ItemInfo.Values.Count(x => x.TemplateID == dictionaryEntryTemplateId);
    }
  }
}