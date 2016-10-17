// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryDataProvider.cs" company="Sitecore">
//   Copyright (c) 2016 Sitecore Corporation A/S. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Runtime.Serialization.Formatters.Binary;
  using System.Threading;
  using Sitecore.Caching;
  using Sitecore.Collections;
  using Sitecore.Data.Generic;
  using Sitecore.Data.Items;
  using Sitecore.Data.Serialization.ObjectModel;
  using Sitecore.Data.Templates;
  using Sitecore.Diagnostics;
  using Sitecore.Eventing;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Extensions.List;
  using Sitecore.Globalization;
  using Sitecore.SecurityModel;
  using Sitecore.Threading;
  using Sitecore.Workflows;

  public sealed partial class MemoryDataProvider : DataProvider
  {
    #region Nested type: FieldsRow

    [Serializable]
    public class FieldsRow
    {
      #region Fields

      public ID FieldID;
      public ID ItemID;
      public string Language;
      public string Value;
      public int Version;

      #endregion
    }

    #endregion

    #region Nested type: ItemsRow

    [Serializable]
    private class ItemsRow
    {
      #region Fields

      public ID BranchID;
      public ID ItemID;
      public string Name;
      public ID ParentID;
      public ID TemplateID;

      public readonly List<ID> Children = new List<ID>();

      #endregion
    }

    #endregion

    #region Nested type: PublishQueueRow

    [Serializable]
    public class PublishQueueRow
    {
      #region Fields

      public string Action;
      public DateTime Date;
      public ID ItemID;
      public string Language;
      public int Version;

      #endregion
    }

    #endregion

    #region Fields

    private const int CurrentStateVersion = 3;

    [NotNull]
    private State _State = new State();

    #endregion

    #region Constructors

    public MemoryDataProvider()
    {
    }

    private static int counter;

    [UsedImplicitly]
    [NotNull]
    private string ID { get; }

    [NotNull]
    private static string GetId(string key)
    {
      return key + Interlocked.Increment(ref counter);
    }

    [UsedImplicitly]
    public MemoryDataProvider([NotNull] string filename, [NotNull] string serializationPath)
    {
      Assert.ArgumentNotNullOrEmpty(serializationPath, nameof(serializationPath));
      Assert.ArgumentNotNullOrEmpty(filename, nameof(filename));

      ID = GetId(filename);
      Log.Debug("MemoryDataProvider " + ID + " is starting", this);
      filename = MainUtil.MapPath(filename);
      serializationPath = Path.GetFullPath(Path.Combine(MainUtil.MapPath("/"), serializationPath));

      var started = new AutoResetEvent(false);
      ManagedThreadPool.QueueUserWorkItem(
        a =>
        {
          lock (this)
          {
            started.Set();
            try
            {
              using (new SecurityDisabler())
              {
                lock (string.Intern(filename))
                {
                  if (!LoadFromFile(filename))
                  {
                    LoadTree(serializationPath);
                    SaveToFile(filename);
                  }
                }
              }
            }
            catch (Exception e)
            {
              Log.Error(e.ToString(), this);
            }
          }
        });

      started.WaitOne();
    }

    #endregion

    #region Properties

    #region Public properties

    [NotNull]
    public string Name
    {
      get
      {
        return "Memory";
      }

      set
      {
      }
    }

    #endregion

    #region Private properties

    private Dictionary<ID, ItemsRow> Items
    {
      get
      {
        return _State.items;
      }

      set
      {
        _State.items = value;
      }
    }

    private List<PublishQueueRow> PublishQueue
    {
      get
      {
        return _State._publishQueue;
      }

      set
      {
        _State._publishQueue = value;
      }
    }

    private ListDictionary<ID, FieldsRow> SharedFields
    {
      get
      {
        return _State.sharedFields;
      }

      set
      {
        _State.sharedFields = value;
      }
    }

    private ListDictionary<ID, FieldsRow> UnversionedFields
    {
      get
      {
        return _State.unversionedFields;
      }

      set
      {
        _State.unversionedFields = value;
      }
    }

    private ListDictionary<ID, FieldsRow> VersionedFields
    {
      get
      {
        return _State.versionedFields;
      }

      set
      {
        _State.versionedFields = value;
      }
    }

    private Dictionary<string, string> Properties
    {
      get
      {
        return _State.properties;
      }

      set
      {
        _State.properties = value;
      }
    }

    #endregion

    #endregion

    #region Public methods

    internal void AddPrefetch(object node)
    {
    }

    public override bool AddToPublishQueue(
      [NotNull] ID itemID,
      [NotNull] string action,
      DateTime date,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        PublishQueue.Add(new PublishQueueRow
        {
          ItemID = itemID,
          Action = action,
          Date = date
        });
        return true;
      }
    }

    public override int AddVersion(
      [NotNull] ItemDefinition itemDefinition,
      [NotNull] VersionUri baseVersion,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        var result = -1;

        if (baseVersion.Version.ToInt32() > 0)
        {
          result = CopyVersion(itemDefinition, baseVersion);
        }

        if (result == -1)
        {
          result = AddBlankVersion(itemDefinition, baseVersion.Language);
        }

        return result;
      }
    }

    public override bool ChangeFieldSharing(
      [NotNull] TemplateField fieldDefinition,
      TemplateFieldSharing sharing,
      [NotNull] CallContext context)
    {
      var converter = GetFieldSharingConverter();

      bool result;

      switch (sharing)
      {
        case TemplateFieldSharing.None:
          result = converter.MakeVersioned(fieldDefinition);
          break;

        case TemplateFieldSharing.Shared:
          result = converter.MakeShared(fieldDefinition);
          break;

        case TemplateFieldSharing.Unversioned:
          result = converter.MakeUnversioned(fieldDefinition);
          break;

        default:
          Log.Warn("Unknown sharing type in ChangeFieldSharing: " + sharing, this);
          return false;
      }

      if (result)
      {
        CacheManager.ClearAllCaches();
      }

      return result;
    }

    public override bool ChangeTemplate(
      [NotNull] ItemDefinition itemDefinition,
      [NotNull] TemplateChangeList changeList,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        var changes = changeList.Changes;

        foreach (var change in changes)
        {
          switch (change.Action)
          {
            case TemplateChangeAction.ChangeFieldID:
              ChangeTemplate_ChangeFieldID(change, itemDefinition.ID);
              ChangeTemplate_ChangeFieldSharing(change, itemDefinition.ID);
              break;

            case TemplateChangeAction.DeleteField:
              ChangeTemplate_DeleteFieldID(change, itemDefinition.ID);
              break;
          }
        }

        ChangeTemplate_ChangeTemplateID(itemDefinition.ID, changeList.Target.ID);

        return true;
      }
    }

    public override bool CleanupDatabase([NotNull] CallContext context)
    {
      return true;
    }

    public override bool CleanupPublishQueue(DateTime to, [NotNull] CallContext context)
    {
      lock (this)
      {
        PublishQueue = PublishQueue.Where(item => item.Date > to).ToList();

        return true;
      }
    }

    public override bool CopyItem(
      [NotNull] ItemDefinition source,
      [NotNull] ItemDefinition destination,
      [NotNull] string copyName,
      [NotNull] ID copyID,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        var item = Items[source.ID];
        Items[copyID] = new ItemsRow
        {
          ItemID = copyID,
          Name = item.Name,
          ParentID = destination.ID,
          TemplateID = item.TemplateID,
          BranchID = item.BranchID
        };

        Items[destination.ID].Children.Add(copyID);
        Action<Dictionary<ID, List<FieldsRow>>> copyList =
          dict =>
            dict[copyID] =
              new List<FieldsRow>(
                dict[source.ID].Select(
                  field =>
                    new FieldsRow
                    {
                      FieldID = field.FieldID,
                      ItemID = copyID,
                      Language = field.Language,
                      Version = field.Version,
                      Value = field.Value
                    }));
        copyList(SharedFields);
        copyList(UnversionedFields);
        copyList(VersionedFields);

        return true;
      }
    }

    public override bool CreateItem(
      [NotNull] ID itemID,
      [NotNull] string itemName,
      [NotNull] ID templateID,
      [NotNull] ItemDefinition parent,
      [NotNull] CallContext context)
    {
      Assert.ArgumentNotNullOrEmpty(itemID, nameof(itemID));
      Assert.ArgumentNotNullOrEmpty(itemName, nameof(itemName));
      Assert.ArgumentNotNullOrEmpty(templateID, nameof(templateID));
      Assert.ArgumentNotNull(parent, nameof(parent));
      Assert.ArgumentNotNull(context, nameof(context));

      lock (this)
      {
        if (ItemExists(itemID))
        {
          return true;
        }

        Items[itemID] = new ItemsRow
        {
          ItemID = itemID,
          Name = itemName,
          TemplateID = templateID,
          BranchID = Data.ID.Null,
          ParentID = parent.ID
        };

        ItemsRow parentItem;
        if (Items.TryGetValue(parent.ID, out parentItem))
        {
          parentItem.Children.Add(itemID);
        }

        return true;
      }
    }

    public override bool DeleteItem([NotNull] ItemDefinition itemDefinition, [NotNull] CallContext context)
    {
      lock (this)
      {
        var parentId = GetParentID(itemDefinition, context);

        Items.Remove(itemDefinition.ID);
        SharedFields.Remove(itemDefinition.ID);
        UnversionedFields.Remove(itemDefinition.ID);
        VersionedFields.Remove(itemDefinition.ID);

        Items[parentId].Children.Remove(itemDefinition.ID);

        return true;
      }
    }

    [NotNull]
    public override IDList GetChildIDs([NotNull] ItemDefinition itemDefinition, [NotNull] CallContext context)
    {
      Assert.ArgumentNotNull(itemDefinition, nameof(itemDefinition));
      Assert.ArgumentNotNull(context, nameof(context));
      lock (this)
      {
        var result = new IDList();
        Items[itemDefinition.ID].Children.Apply(result.Add);
        return result;
      }
    }

    public override long GetDataSize(int minEntitySize, int maxEntitySize)
    {
      return 0;
    }

    public override long GetDictionaryEntryCount()
    {
      return 0;
    }

    public override EventQueue GetEventQueue()
    {
      return NullEventQueue.Instance;
    }

    [CanBeNull]
    public override ItemDefinition GetItemDefinition([NotNull] ID itemId, [NotNull] CallContext context)
    {
      Assert.ArgumentNotNull(itemId, nameof(itemId));
      Assert.ArgumentNotNull(context, nameof(context));
      lock (this)
      {
        ItemsRow row;
        if (!Items.TryGetValue(itemId, out row))
        {
          return null;
        }

        var result = new ItemDefinition(row.ItemID, row.Name, row.TemplateID, row.BranchID);
        (result as ICacheable).Cacheable = false;
        return result;
      }
    }

    [NotNull]
    public override FieldList GetItemFields(
      [NotNull] ItemDefinition itemDefinition,
      [NotNull] VersionUri versionUri,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        var result = new FieldList();
        VersionedFields[itemDefinition.ID].Where(
            row => (row.Language == versionUri.Language.ToString()) && (row.Version == versionUri.Version.ToInt32()))
          .Apply(row => result.Add(row.FieldID, row.Value));
        UnversionedFields[itemDefinition.ID].Where(row => row.Language == versionUri.Language.ToString())
          .Apply(row => result.Add(row.FieldID, row.Value));
        SharedFields[itemDefinition.ID].Apply(row => result.Add(row.FieldID, row.Value));
        if (context.CurrentResult == null)
        {
          context.CurrentResult = result;
        }
        return result;
      }
    }

    [NotNull]
    public override VersionUriList GetItemVersions(
      [NotNull] ItemDefinition itemDefinition,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        var list = new VersionUriList();
        VersionedFields[itemDefinition.ID].GroupBy(f => f.Language + f.Version)
          .Select(
            p => new VersionUri(Language.Parse(p.First().Language), Data.Version.Parse(p.First().Version)))
          .Apply(list.Add);
        return list;
      }
    }

    [NotNull]
    public override DataUri[] GetItemsInWorkflowState([NotNull] WorkflowInfo info, [NotNull] CallContext context)
    {
      var result = new List<DataUri>();
      lock (this)
      {
        foreach (var list in VersionedFields.Values)
        {
          foreach (
            var f in
            list.Where(f => (f.FieldID == FieldIDs.WorkflowState) && (f.Value == info.StateID.ToString())))
          {
            result.Add(new DataUri(f.ItemID, Language.Parse(f.Language), Data.Version.Parse(f.Version)));
          }
        }
      }
      return result.ToArray();
    }

    [NotNull]
    public LanguageCollection GetLanguages()
    {
      lock (this)
      {
        var languages = new List<Language>();
        foreach (var item in Items.Values.Where(i => i.TemplateID == TemplateIDs.Language))
        {
          var language = Language.Parse(item.Name);
          language.Origin.ItemId = item.ItemID;
          languages.Add(language);
        }

        return new LanguageCollection(languages);
      }
    }

    [NotNull]
    public override LanguageCollection GetLanguages([NotNull] CallContext context)
    {
      return GetLanguages();
    }

    public override ID GetParentID([NotNull] ItemDefinition itemDefinition, [NotNull] CallContext context)
    {
      lock (this)
      {
        return Items[itemDefinition.ID].ParentID;
      }
    }

    public override string GetProperty([NotNull] string propertyName, [NotNull] CallContext context)
    {
      lock (this)
      {
        string result;
        return Properties.TryGetValue(propertyName, out result) ? result : string.Empty;
      }
    }

    [NotNull]
    public override List<string> GetPropertyKeys([NotNull] string prefix, [NotNull] CallContext context)
    {
      lock (this)
      {
        return Properties.Keys.ToList();
      }
    }

    [NotNull]
    public override IDList GetPublishQueue(DateTime from, DateTime to, [NotNull] CallContext context)
    {
      lock (this)
      {
        var result = new IDList();
        PublishQueue.Where(r => (r.Date >= from) && (r.Date <= to)).Apply(r => result.Add(r.ItemID));
        return result;
      }
    }

    public override ID GetRootID([NotNull] CallContext context)
    {
      return ItemIDs.RootID;
    }

    public DefaultFieldSharing.SharingType GetSharingType([NotNull] TemplateField templateField)
    {
      if (templateField == null)
      {
        return DefaultFieldSharing.SharingType.Unknown;
      }

      if (templateField.IsShared)
      {
        return DefaultFieldSharing.SharingType.Shared;
      }

      if (templateField.IsUnversioned)
      {
        return DefaultFieldSharing.SharingType.Unversioned;
      }

      if (templateField.IsVersioned)
      {
        return DefaultFieldSharing.SharingType.Versioned;
      }

      return DefaultFieldSharing.SharingType.Unknown;
    }

    [NotNull]
    public override IdCollection GetTemplateItemIds([NotNull] CallContext context)
    {
      var result = new IdCollection();
      lock (this)
      {
        Items.Values.Where(r => r.TemplateID == TemplateIDs.Template).Apply(r => result.Add(r.ItemID));
      }
      return result;
    }

    public override bool HasChildren([NotNull] ItemDefinition itemDefinition, [NotNull] CallContext context)
    {
      lock (this)
      {
        return Items.Values.Any(r => r.ParentID == itemDefinition.ID);
      }
    }

    public override bool MoveItem(
      [NotNull] ItemDefinition itemDefinition,
      [NotNull] ItemDefinition destination,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        var item = Items[itemDefinition.ID];
        Items[item.ParentID].Children.Remove(item.ItemID);

        item.ParentID = destination.ID;
        Items[item.ParentID].Children.Add(item.ItemID);
      }

      return true;
    }

    public override void RemoveLanguageData([NotNull] Language language, [NotNull] CallContext context)
    {
      lock (this)
      {
        RemoveLanguageData(language.Name);
      }
    }

    public override bool RemoveProperty([NotNull] string propertyName, bool isPrefix, [NotNull] CallContext context)
    {
      lock (this)
      {
        Properties.Remove(propertyName);
      }

      return true;
    }

    public override bool RemoveVersion(
      [NotNull] ItemDefinition itemDefinition,
      [NotNull] VersionUri version,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        VersionedFields[itemDefinition.ID] =
          VersionedFields[itemDefinition.ID].Where(
              r => (r.Language != version.Language.ToString()) || (r.Version != version.Version.ToInt32()))
            .ToList();
      }

      return true;
    }

    public override bool RemoveVersions(
      [NotNull] ItemDefinition itemDefinition,
      [NotNull] Language language,
      bool removeSharedData,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        if (language == Language.Invariant)
        {
          RemoveAllVersions(itemDefinition);
        }
        else
        {
          RemoveLanguageVersions(itemDefinition, language, removeSharedData);
        }
      }

      return true;
    }

    public override void RenameLanguageData(
      [NotNull] string fromLanguage,
      [NotNull] string toLanguage,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        UnversionedFields.Values.Apply(
          list => list.Where(r => r.Language == fromLanguage).Apply(r => r.Language = toLanguage));
        VersionedFields.Values.Apply(
          list => list.Where(r => r.Language == fromLanguage).Apply(r => r.Language = toLanguage));
      }
    }

    public override bool SaveItem(
      [NotNull] ItemDefinition itemDefinition,
      [NotNull] ItemChanges changes,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        if (changes.HasPropertiesChanged || changes.HasFieldsChanged)
        {
          if (changes.HasPropertiesChanged)
          {
            UpdateItemDefinition(itemDefinition, changes);
          }

          if (changes.HasFieldsChanged)
          {
            UpdateItemFields(itemDefinition.ID, changes);
          }
        }
      }

      return true;
    }

    public override bool SetProperty(
      [NotNull] string parameterName,
      [NotNull] string value,
      [NotNull] CallContext context)
    {
      lock (this)
      {
        Properties[parameterName] = value;
      }
      return true;
    }

    public bool LoadFromFile([NotNull] string name)
    {
      lock (this)
      {
        try
        {
          name = MainUtil.MapPath(name);
          if (!File.Exists(name))
          {
            return false;
          }

          var formatter = new BinaryFormatter();

          using (var stream = File.OpenRead(name))
          {
            var loadedState = (State)formatter.Deserialize(stream);
            if (CurrentStateVersion == loadedState.version)
            {
              _State = loadedState;
              return true;
            }

            return false;
          }
        }
        catch (Exception e)
        {
          Log.Error("Memory Data Provider - Failed to load from cache: ", e, this);
          return false;
        }
      }
    }

    public void SaveToFile([NotNull] string name)
    {
      lock (this)
      {
        _State.version = CurrentStateVersion;
        using (var stream = File.Create(name))
        {
          var formatter = new BinaryFormatter();
          formatter.Serialize(stream, _State);
        }
      }
    }

    #endregion

    #region Methods

    private void LoadTree(string serializationPath)
    {
      foreach (var file in Directory.GetFiles(serializationPath))
      {
        if (!SerializationUtils.IsItemSerialization(file))
        {
          continue;
        }

        LoadItem(file);
      }

      foreach (var directory in Directory.GetDirectories(serializationPath))
      {
        LoadTree(directory);
      }
    }

    private void LoadItem(string file)
    {
      using (var reader = File.OpenText(file))
      {
        var item = SyncItem.ReadItem(new Tokenizer(reader));

        var itemId = new ID(item.ID);
        var parentId = new ID(item.ParentID);
        var templateId = new ID(item.TemplateID);
        var branchId = new ID(item.BranchId);

        if (!Items.ContainsKey(itemId))
        {
          Items[itemId] = new ItemsRow
          {
            ItemID = itemId,
            Name = item.Name,
            ParentID = parentId,
            BranchID = branchId,
            TemplateID = templateId
          };
        }

        Items[itemId].ParentID = parentId;
        Items[itemId].TemplateID = templateId;
        Items[itemId].BranchID = branchId;
        Items[itemId].Name = item.Name;

        if (parentId != Data.ID.Null)
        {
          if (!Items.ContainsKey(parentId))
          {
            Items[parentId] = new ItemsRow
            {
              ItemID = parentId,
              Name = "(unknown)",
              ParentID = Data.ID.Null,
              BranchID = Data.ID.Null,
              TemplateID = Data.ID.Null
            };
          }

          Items[parentId].Children.Add(itemId);
        }

        foreach (var field in item.SharedFields)
        {
          var fieldId = new ID(field.FieldID);
          SharedFields[itemId].Add(
            new FieldsRow
            {
              ItemID = itemId,
              FieldID = fieldId,
              Value = field.FieldValue
            });
        }

        foreach (var version in item.Versions)
        {
          var versionNumber = MainUtil.GetInt(version.Version, 0);
          foreach (var field in version.Fields)
          {
            var fieldId = new ID(field.FieldID);
            VersionedFields[itemId].Add(
              new FieldsRow
              {
                ItemID = itemId,
                FieldID = fieldId,
                Language = version.Language,
                Version = versionNumber,
                Value = field.FieldValue
              });
          }
        }
      }
    }

    private int AddBlankVersion([NotNull] ItemDefinition item, [NotNull] Language language)
    {
      var newVersion = GetLatestVersion(item, language) + 1;

      VersionedFields[item.ID].Add(
        new FieldsRow
        {
          ItemID = item.ID,
          Language = language.ToString(),
          Version = newVersion,
          FieldID = FieldIDs.Created,
          Value = DateUtil.IsoNowWithTicks
        });

      return newVersion;
    }

    private void ChangeTemplate_ChangeFieldID(
      [NotNull] TemplateChangeList.TemplateChange change,
      [NotNull] ID itemId)
    {
      if (change.SourceField.ID == change.TargetField.ID)
      {
        return;
      }

      ChangeTemplate_ChangeFieldValues(itemId, change.SourceField.ID, change.TargetField.ID);
    }

    private void ChangeTemplate_ChangeFieldSharing(
      [NotNull] TemplateChangeList.TemplateChange change,
      [NotNull] ID itemID)
    {
      GetFieldSharingConverter().MoveFieldData(change.SourceField, change.TargetField, itemID);
    }

    private void ChangeTemplate_ChangeFieldValues(
      [NotNull] ID itemId,
      [NotNull] ID oldFieldId,
      [NotNull] ID newFieldId)
    {
      new[] {SharedFields.Values, UnversionedFields.Values, VersionedFields.Values}.Apply(
        c =>
          c.Apply(
            list =>
              list.Where(r => (r.ItemID == itemId) && (r.FieldID == oldFieldId))
                .Apply(r => r.FieldID = newFieldId)));
    }

    private void ChangeTemplate_ChangeTemplateID([NotNull] ID itemID, [NotNull] ID templateID)
    {
      Items[itemID].TemplateID = templateID;
    }

    private void ChangeTemplate_DeleteFieldID(
      [NotNull] TemplateChangeList.TemplateChange change,
      [NotNull] ID itemId)
    {
      Action<List<FieldsRow>> delete =
        list => { list.Where(r => r.FieldID == change.SourceField.ID).ToArray().Apply(r => list.Remove(r)); };

      SharedFields.Values.Apply(delete);
      UnversionedFields.Values.Apply(delete);
      VersionedFields.Values.Apply(delete);
    }

    private int CopyVersion([NotNull] ItemDefinition item, [NotNull] VersionUri baseVersion)
    {
      if (!VersionExists(item, baseVersion))
      {
        return -1;
      }

      var newVersion = GetLatestVersion(item, baseVersion.Language) + 1;

      VersionedFields[item.ID].AddRange(
        VersionedFields[item.ID].Where(
            r => (r.Language == baseVersion.Language.ToString()) && (r.Version == baseVersion.Version.ToInt32()))
          .Select(
            r =>
              new FieldsRow
              {
                ItemID = item.ID,
                Language = r.Language,
                Version = newVersion,
                FieldID = r.FieldID,
                Value = r.Value
              }));

      return newVersion;
    }

    [NotNull]
    private FieldSharingConverter GetFieldSharingConverter()
    {
      return new FieldSharingConverter(this);
    }

    private int GetLatestVersion([NotNull] ItemDefinition item, [NotNull] Language language)
    {
      return
        VersionedFields[item.ID].Where(r => (r.ItemID == item.ID) && (r.Language == language.ToString()))
          .Select(r => r.Version)
          .Concat(new[] {0})
          .Max();
    }

    private DefaultFieldSharing.SharingType GetSharingType([NotNull] FieldChange change)
    {
      var definition = change.Definition;

      if (definition == null)
      {
        return DefaultFieldSharing.Sharing[change.FieldID];
      }

      return GetSharingType(definition);
    }

    [NotNull]
    private List<Data.Version> GetVersions()
    {
      return
        VersionedFields.Values.Cast<IEnumerable<FieldsRow>>()
          .Aggregate((a, b) => a.Concat(b))
          .GroupBy(r => r.Version)
          .Select(g => new Data.Version(g.Key))
          .ToList();
    }

    private bool ItemExists([NotNull] ID itemId)
    {
      return Items.ContainsKey(itemId);
    }

    private void RemoveAllVersions([NotNull] ItemDefinition item)
    {
      VersionedFields.Remove(item.ID);
    }

    private void RemoveField([NotNull] ID itemId, [NotNull] FieldChange change)
    {
      var sharing = GetSharingType(change);

      if ((sharing == DefaultFieldSharing.SharingType.Versioned)
          || (sharing == DefaultFieldSharing.SharingType.Unknown))
      {
        VersionedFields[itemId].Delete(
          r =>
            (r.Language == change.Language.ToString()) && (r.Version == change.Version.ToInt32())
            && (r.FieldID == change.FieldID));
      }

      if ((sharing == DefaultFieldSharing.SharingType.Shared) || (sharing == DefaultFieldSharing.SharingType.Unknown))
      {
        SharedFields[itemId].Delete(r => r.FieldID == change.FieldID);
      }

      if ((sharing == DefaultFieldSharing.SharingType.Unversioned)
          || (sharing == DefaultFieldSharing.SharingType.Unknown))
      {
        UnversionedFields[itemId].Delete(
          r => (r.Language == change.Language.ToString()) && (r.FieldID == change.FieldID));
      }
    }

    private void RemoveFields([NotNull] ID itemId, [NotNull] Language language, [NotNull] Data.Version version)
    {
      SharedFields.Remove(itemId);
      {
        var itemFields = UnversionedFields[itemId];
        itemFields.Where(r => r.Language == language.ToString()).ToList().Apply(r => itemFields.Remove(r));
      }
      {
        var itemFields = VersionedFields[itemId];
        itemFields.Where(r => (r.Language == language.ToString()) && (r.Version == version.ToInt32()))
          .ToList()
          .Apply(r => itemFields.Remove(r));
      }
    }

    private void RemoveLanguageData([NotNull] string languageName)
    {
      UnversionedFields.Values.Apply(list => list.Delete(r => r.Language == languageName));
      VersionedFields.Values.Apply(list => list.Delete(r => r.Language == languageName));
    }

    private void RemoveLanguageVersions(
      [NotNull] ItemDefinition item,
      [NotNull] Language language,
      bool removeSharedData)
    {
      if (removeSharedData)
      {
        SharedFields.Remove(item.ID);
      }

      UnversionedFields[item.ID].Delete(r => r.Language == language.ToString());
      VersionedFields[item.ID].Delete(r => r.Language == language.ToString());
    }

    private void UpdateItemDefinition([NotNull] ItemDefinition item, [NotNull] ItemChanges changes)
    {
      var itemName = StringUtil.GetString(changes.GetPropertyValue("name"), item.Name);
      var templateID = MainUtil.GetObject(changes.GetPropertyValue("templateid"), item.TemplateID) as ID;
      var branchId = MainUtil.GetObject(changes.GetPropertyValue("branchid"), item.BranchId) as ID;

      ItemsRow r;

      if (!Items.TryGetValue(item.ID, out r))
      {
        return;
      }

      r.Name = itemName;
      r.TemplateID = templateID;
      r.BranchID = branchId;
    }

    private void UpdateItemFields([NotNull] ID itemId, [NotNull] ItemChanges changes)
    {
      Assert.ArgumentNotNull(itemId, "value");
      lock ((object)this)
      {
        var now = DateTime.Now;

        var fullUpdate = changes.Item.RuntimeSettings.SaveAll;
        if (fullUpdate)
        {
          RemoveFields(itemId, changes.Item.Language, changes.Item.Version);
        }

        var updateOrder = new[]
        {
          DefaultFieldSharing.SharingType.Shared, DefaultFieldSharing.SharingType.Unversioned,
          DefaultFieldSharing.SharingType.Versioned
        };
        foreach (var sharingType in updateOrder)
        {
          foreach (FieldChange change in changes.FieldChanges)
          {
            if (GetSharingType(change) != sharingType)
            {
              continue;
            }

            if (change.RemoveField)
            {
              if (!fullUpdate)
              {
                RemoveField(itemId, change);
              }
            }
            else
            {
              switch (sharingType)
              {
                case DefaultFieldSharing.SharingType.Shared:
                  WriteSharedField(itemId, change, now, fullUpdate);
                  break;

                case DefaultFieldSharing.SharingType.Unversioned:
                  WriteUnversionedField(itemId, change, now, fullUpdate);
                  break;

                case DefaultFieldSharing.SharingType.Versioned:
                  WriteVersionedField(itemId, change, now, fullUpdate);
                  break;
              }
            }
          }
        }
      }
    }

    private bool VersionExists([NotNull] ItemDefinition item, [NotNull] VersionUri versionUri)
    {
      return
        VersionedFields[item.ID].Any(
          r =>
            (r.ItemID == item.ID) && (r.Language == versionUri.Language.ToString())
            && (r.Version == versionUri.Version.ToInt32()));
    }

    private void WriteSharedField(
      [NotNull] ID itemId,
      [NotNull] FieldChange change,
      DateTime now,
      bool fieldsAreEmpty)
    {
      var row = fieldsAreEmpty
        ? null
        : SharedFields[itemId].Where(r => r.FieldID == change.FieldID).FirstOrDefault();
      if (row == null)
      {
        SharedFields[itemId].Add(
          new FieldsRow
          {
            ItemID = itemId,
            FieldID = change.FieldID,
            Value = change.Value
          });
      }
      else
      {
        row.Value = change.Value;
      }
    }

    private void WriteUnversionedField(
      [NotNull] ID itemId,
      [NotNull] FieldChange change,
      DateTime now,
      bool fieldsAreEmpty)
    {
      var row = fieldsAreEmpty
        ? null
        : UnversionedFields[itemId].Where(
          r => (r.Language == change.Language.ToString()) && (r.FieldID == change.FieldID)).FirstOrDefault();
      if (row == null)
      {
        UnversionedFields[itemId].Add(
          new FieldsRow
          {
            ItemID = itemId,
            Language = change.Language.ToString(),
            FieldID = change.FieldID,
            Value = change.Value
          });
      }
      else
      {
        row.Value = change.Value;
      }
    }

    private void WriteVersionedField(
      [NotNull] ID itemId,
      [NotNull] FieldChange change,
      DateTime now,
      bool fieldsAreEmpty)
    {
      var row = fieldsAreEmpty
        ? null
        : VersionedFields[itemId].Where(
          r =>
            (r.Language == change.Language.ToString()) && (r.Version == change.Version.ToInt32())
            && (r.FieldID == change.FieldID)).FirstOrDefault();
      if (row == null)
      {
        VersionedFields[itemId].Add(
          new FieldsRow
          {
            ItemID = itemId,
            Language = change.Language.ToString(),
            Version = change.Version.ToInt32(),
            FieldID = change.FieldID,
            Value = change.Value
          });
      }
      else
      {
        row.Value = change.Value;
      }
    }

    #endregion
  }
}