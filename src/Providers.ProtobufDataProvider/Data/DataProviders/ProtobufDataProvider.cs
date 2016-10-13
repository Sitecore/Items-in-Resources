namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Sitecore.Data.DataAccess;
  using Sitecore.Data.DataFormat;
  using Sitecore;
  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Diagnostics;
  using Sitecore.Exceptions;
  using Sitecore.Globalization;

  public class ProtobufDataProvider : DataProvider
  {
    private readonly DataSet DataSet;

    [NotNull]
    private readonly string DatabaseName;

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

    public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
    {
      Assert.ArgumentNotNull(itemId, nameof(itemId));
      Assert.ArgumentNotNull(context, nameof(context));

      ItemInfo item;
      if (!DataSet.ItemInfo.TryGetValue(itemId.Guid, out item))
      {
        return null;
      }

      context.Abort();
      return new ItemDefinition(itemId, item.Name, ID.Parse(item.TemplateID), ID.Null);
    }

    public override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
    {
      // TODO: change signature to create IDList once per all data providers

      var list = new IDList();
      ItemInfo[] array;
      if (!DataSet.Children.TryGetValue(itemDefinition.ID.Guid, out array))
      {
        return list;
      }

      for (int i = 0; i < array.Length; i++)
      {
        var item = array[i];
        list.Add(ID.Parse(item.ID));
      }

      context.Abort();
      return list;
    }

    public override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
    {
      ItemInfo item;
      if (!DataSet.ItemInfo.TryGetValue(itemDefinition.ID.Guid, out item))
      {
        return null;
      }

      context.Abort();
      return ID.Parse(item.ParentID);
    }

    public override VersionUriList GetItemVersions(ItemDefinition itemDefinition, CallContext context)
    {
      // TODO: change signature to create FieldList once per all data providers
      var list = new VersionUriList();

      ItemLanguagesData item;
      if (!DataSet.LanguageData.TryGetValue(itemDefinition.ID.Guid, out item))
      {
        return list;
      }

      foreach (var lang in item.Keys)
      {
        // in read-only data provider only single "1" version per language is supported
        list.Add(Language.Parse(lang), Sitecore.Data.Version.Parse(1));
      }

      context.Abort();
      return list;
    }

    public override FieldList GetItemFields(ItemDefinition itemDefinition, VersionUri versionUri, CallContext context)
    {
      // TODO: change signature to create FieldList once per all data providers

      var list = new FieldList();
      if (versionUri.Version.Number > 1)
      {
        return list;
      }

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

      context.Abort();
      return list;
    }

    public override ID ResolvePath(string itemPath, CallContext context)
    {
      Guid id;
      if (!Guid.TryParse(itemPath, out id) && !ItemPathResolver.TryResolvePath(itemPath, DataSet.Children, out id))
      {
        // TODO: remove that after fixing null ref in SqlDataProvider.ResolvePathRec
        context.Abort();
        return null;
      }

      context.Abort();
      return ID.Parse(id);
    }

    public override ID SelectSingleID(string query, CallContext context)
    {
      var ids = DoSelectIDs(query);
      var result = ids.Count > 0 ? ids[0] : null;

      Log.Info($"SelectSingleID: {query}, Value: {result}", this);
      context.Abort();
      return result;
    }                             

    private static readonly Regex DescendantsQueryRegex = new Regex(@"^fast\://\*(\[[^\]])$", RegexOptions.Compiled);

    public override IDList SelectIDs(string query, CallContext context)
    {
      var list = DoSelectIDs(query);

      Log.Info($"SelectID: {query}, Values: {string.Join(", ", list.ToArray())}", this);
      context.Abort();
      return list;
    }

    private IDList DoSelectIDs(string query)
    {
      // 18860 18:03:22 INFO  SelectSingleID: fast://*[@@templateid = '{F68F13A6-3395-426A-B9A1-FA2DC60D94EB}' and @@name = 'da']

      var desc = DescendantsQueryRegex.Match(query);
      var list = new IDList();
      if (desc.NextMatch().Success)
      {
        var conditionsQueryGroup = desc.Groups[1];
        var items = (IEnumerable<ItemInfo>)DataSet.ItemInfo.Values;
        if (conditionsQueryGroup.Success)
        {
          var conditionsQuery = conditionsQueryGroup.Value;
          if (string.IsNullOrEmpty(conditionsQuery))
            throw new QueryException("Query must not contain empty square bracets []");

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

        items.Take(100).Select(x => x.ID).ToList().ForEach(x => list.Add(ID.Parse(x)));
      }

      return list;
    }

    public override bool HasChildren(ItemDefinition itemDefinition, CallContext context)
    {
      ItemInfo[] children;

      return DataSet.Children.TryGetValue(itemDefinition.ID.Guid, out children) && children != null && children.Length > 0;
    }

    public override IdCollection GetTemplateItemIds(CallContext context)
    {
      var templateId = TemplateIDs.Template.Guid;
      var templates = DataSet.ItemInfo.Values.Where(x => x.TemplateID == templateId);
      var ids = new IdCollection();
      foreach (var template in templates)
      {
        ids.Add(ID.Parse(template.ID));
      }

      return ids;
    }

    public override LanguageCollection GetLanguages(CallContext context)
    {
      var languages = DataSet.Children[Data.ItemIDs.LanguagesRootId]?
        .Select(x => x?.Name)
        .Where(x => !string.IsNullOrEmpty(x))
        .Select(x => Language.Parse(x)) ?? new Language[0];

      context.Abort();
      return new LanguageCollection(languages);
    }

    public override long GetDictionaryEntryCount()
    {
      var dictionaryEntryTemplateId = TemplateIDs.DictionaryEntry.Guid;

      return DataSet.ItemInfo.Values.Count(x => x.TemplateID == dictionaryEntryTemplateId);
    }
  }
}