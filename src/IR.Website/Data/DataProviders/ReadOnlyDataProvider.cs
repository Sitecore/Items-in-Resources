namespace IR.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using IR.Data.DataAccess;
  using IR.Data.DataFormat;
  using Sitecore;
  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.DataProviders;
  using Sitecore.Data.Templates;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;

  public class ReadOnlyDataProvider : DataProvider
  {
    private readonly DataSet DataSet;

    [NotNull]
    private readonly string DatabaseName;

    [UsedImplicitly]
    public ReadOnlyDataProvider([NotNull] string databaseName, [NotNull] string definitionsFilePath, [NotNull] string sharedDataFilePath, [NotNull] string languageDataFilePath)
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
      if (!this.DataSet.ItemInfo.TryGetValue(itemId.Guid, out item))
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
      if (!this.DataSet.Children.TryGetValue(itemDefinition.ID.Guid, out array))
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
      if (!this.DataSet.ItemInfo.TryGetValue(itemDefinition.ID.Guid, out item))
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
      if (!this.DataSet.LanguageData.TryGetValue(itemDefinition.ID.Guid, out item))
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

      Dictionary<Guid, string> sharedFields;
      if (this.DataSet.SharedData.TryGetValue(itemDefinition.ID.Guid, out sharedFields))
      {
        foreach (var sharedField in sharedFields)
        {
          list.Add(ID.Parse(sharedField.Key), sharedField.Value ?? string.Empty);
        }
      }

      ItemLanguagesData languages;
      if (this.DataSet.LanguageData.TryGetValue(itemDefinition.ID.Guid, out languages))
      {
        foreach (var pair in languages)
        {
          if (!pair.Key.Equals(versionUri.Language.Name, StringComparison.OrdinalIgnoreCase))
          {
            continue;
          }

          foreach (var languageField in pair.Value ?? Empty.Fields)
          {
            list.Add(ID.Parse(languageField.Key), languageField.Value ?? string.Empty);
          }

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
      throw new NotImplementedException("Fast Query and Sitecore Query are not yet implemented");
    }

    public override bool HasChildren(ItemDefinition itemDefinition, CallContext context)
    {
      ItemInfo[] children;
      return this.DataSet.Children.TryGetValue(itemDefinition.ID.Guid, out children) && children != null && children.Length > 0;
    }

    public override IdCollection GetTemplateItemIds(CallContext context)
    {
      var templateId = TemplateIDs.Template.Guid;
      var templates = this.DataSet.ItemInfo.Values.Where(x => x.TemplateID == templateId);
      var ids = new IdCollection();
      foreach (var template in templates)
      {
        ids.Add(ID.Parse(template.ID));
      }

      return ids;
    }

    public override ID GetRootID(CallContext context)
    {
      return base.GetRootID(context);
    }

    public override TemplateCollection GetTemplates(CallContext context)
    {
      return base.GetTemplates(context);
    }

    public override LanguageCollection GetLanguages(CallContext context)
    {
      context.Abort();
      return new LanguageCollection(DataSet.Children[Guid.Parse("{64C4F646-A3FA-4205-B98E-4DE2C609B60F}")]?.Select(x => x?.Name).Where(x => !string.IsNullOrEmpty(x)).Select(x => Language.Parse(x)) ?? new Language[0]);
    }

    public override long GetDictionaryEntryCount()
    {
      var dictionaryEntryTemplateId = TemplateIDs.DictionaryEntry.Guid;

      return this.DataSet.ItemInfo.Values?.Count(x => x.TemplateID == dictionaryEntryTemplateId) ?? 0;
    }
  }
}