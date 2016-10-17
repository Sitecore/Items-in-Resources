namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Data.Templates;
  using Sitecore.Diagnostics;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Extensions.List;

  public partial class MemoryDataProvider
  {
    public sealed class FieldSharingConverter
    {
      #region Constructors

      public FieldSharingConverter([NotNull] MemoryDataProvider owner)
      {
        Assert.ArgumentNotNull(owner, nameof(owner));

        Owner = owner;
      }

      #endregion

      #region Properties

      private MemoryDataProvider Owner { get; }

      #endregion

      #region Public Methods

      public bool ChangeShareType(
          [NotNull] ID fieldId,
          DefaultFieldSharing.SharingType sourceType,
          DefaultFieldSharing.SharingType targetType)
      {
        if (sourceType == targetType)
        {
          return false;
        }

        if (targetType == DefaultFieldSharing.SharingType.Shared)
        {
          return MakeShared(fieldId, sourceType);
        }

        if (targetType == DefaultFieldSharing.SharingType.Unversioned)
        {
          return MakeUnversioned(fieldId, sourceType);
        }

        if (targetType == DefaultFieldSharing.SharingType.Versioned)
        {
          return MakeVersioned(fieldId, sourceType);
        }

        throw new InvalidOperationException("Unknown sharing type.");
      }

      public bool MakeShared([NotNull] TemplateField fieldDefinition)
      {
        Assert.ArgumentNotNull(fieldDefinition, nameof(fieldDefinition));

        return MakeShared(fieldDefinition.ID, Owner.GetSharingType(fieldDefinition));
      }

      public bool MakeShared([NotNull] ID fieldId, DefaultFieldSharing.SharingType sharingType)
      {
        if (sharingType == DefaultFieldSharing.SharingType.Shared)
        {
          return false;
        }

        if (sharingType == DefaultFieldSharing.SharingType.Unversioned)
        {
          return MakeSharedFromUnversioned(fieldId);
        }

        if (sharingType == DefaultFieldSharing.SharingType.Versioned)
        {
          return MakeSharedFromVersioned(fieldId);
        }

        throw new InvalidOperationException("Unknown sharing type.");
      }

      public bool MakeUnversioned([NotNull] TemplateField fieldDefinition)
      {
        Assert.ArgumentNotNull(fieldDefinition, nameof(fieldDefinition));

        return MakeUnversioned(fieldDefinition.ID, Owner.GetSharingType(fieldDefinition));
      }

      public bool MakeUnversioned([NotNull] ID fieldId, DefaultFieldSharing.SharingType sharingType)
      {
        if (sharingType == DefaultFieldSharing.SharingType.Unversioned)
        {
          return false;
        }

        if (sharingType == DefaultFieldSharing.SharingType.Shared)
        {
          return MakeUnversionedFromShared(fieldId);
        }

        if (sharingType == DefaultFieldSharing.SharingType.Versioned)
        {
          return MakeUnversionedFromVersioned(fieldId);
        }

        throw new InvalidOperationException("Unknown sharing type.");
      }

      public bool MakeVersioned([NotNull] TemplateField fieldDefinition)
      {
        Assert.ArgumentNotNull(fieldDefinition, nameof(fieldDefinition));

        return MakeVersioned(fieldDefinition.ID, Owner.GetSharingType(fieldDefinition));
      }

      public bool MakeVersioned([NotNull] ID fieldId, DefaultFieldSharing.SharingType sharingType)
      {
        if (sharingType == DefaultFieldSharing.SharingType.Versioned)
        {
          return false;
        }

        if (sharingType == DefaultFieldSharing.SharingType.Shared)
        {
          return MakeVersionedFromShared(fieldId);
        }

        if (sharingType == DefaultFieldSharing.SharingType.Unversioned)
        {
          return MakeVersionedFromUnversioned(fieldId);
        }

        throw new InvalidOperationException("Unknown sharing type.");
      }

      public bool MoveFieldData(
          [NotNull] TemplateField sourceField,
          [NotNull] TemplateField targetField,
          [NotNull] ID itemID)
      {
        if (targetField.IsShared)
        {
          if (sourceField.IsVersioned)
          {
            return MoveDataToSharedFromVersioned(targetField.ID, itemID);
          }

          if (sourceField.IsUnversioned)
          {
            return MoveDataToSharedFromUnversioned(targetField.ID, itemID);
          }

          return false;
        }

        if (targetField.IsUnversioned)
        {
          if (sourceField.IsVersioned)
          {
            return MoveDataToUnversionedFromVersioned(targetField.ID, itemID);
          }

          if (sourceField.IsShared)
          {
            return MoveDataToUnversionedFromShared(targetField.ID, itemID);
          }

          return false;
        }

        if (sourceField.IsShared)
        {
          return MoveDataToVersionedFromShared(targetField.ID, itemID);
        }

        if (sourceField.IsUnversioned)
        {
          return MoveDataToVersionedFromUnversioned(targetField.ID, itemID);
        }

        return false;
      }

      #endregion

      #region Methods

      private void DeleteSharedFields([NotNull] ID fieldId, [NotNull] ID itemId)
      {
        if (Data.ID.IsNullOrEmpty(itemId))
        {
          Owner.SharedFields.Values.Apply(list => list.Delete(r => r.FieldID == fieldId));
        }
        else
        {
          Owner.SharedFields[itemId].Delete(r => r.FieldID == fieldId);
        }
      }

      private void DeleteUnversionedFields([NotNull] ID fieldId, [NotNull] ID itemId)
      {
        if (Data.ID.IsNullOrEmpty(itemId))
        {
          Owner.UnversionedFields.Values.Apply(list => list.Delete(r => r.FieldID == fieldId));
        }
        else
        {
          Owner.UnversionedFields[itemId].Delete(r => r.FieldID == fieldId);
        }
      }

      private void DeleteVersionedFields([NotNull] ID fieldId, [NotNull] ID itemId)
      {
        if (Data.ID.IsNullOrEmpty(itemId))
        {
          Owner.VersionedFields.Values.Apply(list => list.Delete(r => r.FieldID == fieldId));
        }
        else
        {
          Owner.VersionedFields[itemId].Delete(r => r.FieldID == fieldId);
        }
      }

      private bool MakeSharedFromUnversioned([NotNull] ID fieldId)
      {
        MoveDataToSharedFromUnversioned(fieldId, Data.ID.Null);

        return true;
      }

      private bool MakeSharedFromVersioned([NotNull] ID fieldId)
      {
        MoveDataToSharedFromVersioned(fieldId, Data.ID.Null);

        return true;
      }

      private bool MakeUnversionedFromShared([NotNull] ID fieldId)
      {
        MoveDataToUnversionedFromShared(fieldId, Data.ID.Null);

        return true;
      }

      private bool MakeUnversionedFromVersioned([NotNull] ID fieldId)
      {
        MoveDataToUnversionedFromVersioned(fieldId, Data.ID.Null);

        return true;
      }

      private bool MakeVersionedFromShared([NotNull] ID fieldId)
      {
        MoveDataToVersionedFromShared(fieldId, Data.ID.Null);

        return true;
      }

      private bool MakeVersionedFromUnversioned([NotNull] ID fieldId)
      {
        MoveDataToVersionedFromUnversioned(fieldId, Data.ID.Null);

        return true;
      }

      private bool MoveDataToSharedFromUnversioned([NotNull] ID fieldId, [NotNull] ID itemId)
      {
        lock (Owner)
        {
          DeleteSharedFields(fieldId, itemId);

          IEnumerable<FieldsRow> rows;

          if (itemId != Data.ID.Null)
          {
            rows = Owner.UnversionedFields[itemId].Where(r => r.FieldID == fieldId);
          }
          else
          {
            rows =
                Owner.UnversionedFields.Values.Select(list => list.Where(r => r.FieldID == fieldId))
                    .Aggregate((a, b) => a.Concat(b));

            rows =
                rows.GroupBy(r => r.ItemID.ToString())
                    .Select(g => g.First(r => r.Language == g.Max(r1 => r1.Language)));
          }

          rows = rows.Select(r => new FieldsRow { ItemID = r.ItemID, FieldID = r.FieldID, Value = r.Value });

          foreach (var row in rows)
          {
            Owner.SharedFields[row.ItemID].Add(row);
          }

          DeleteUnversionedFields(fieldId, itemId);
          DeleteVersionedFields(fieldId, itemId);

          return true;
        }
      }

      private bool MoveDataToSharedFromVersioned([NotNull] ID fieldId, [NotNull] ID itemId)
      {
        lock (Owner)
        {
          DeleteSharedFields(fieldId, itemId);
          DeleteUnversionedFields(fieldId, itemId);

          IEnumerable<FieldsRow> rows;

          if (itemId != Data.ID.Null)
          {
            rows = Owner.VersionedFields[itemId].Where(f => f.FieldID == fieldId);
          }
          else
          {
            rows =
                Owner.VersionedFields.Values.Select(l => l.Where(f => f.FieldID == fieldId))
                    .Aggregate((a, b) => a.Concat(b));
            rows =
                rows.GroupBy(r => r.ItemID.ToString())
                    .Select(
                        g => g.First(r => r.Language + r.Version == g.Max(r1 => r1.Language + r1.Version)));
          }

          rows = rows.Select(r => new FieldsRow { ItemID = r.ItemID, FieldID = r.FieldID, Value = r.Value });

          foreach (FieldsRow row in rows)
          {
            Owner.SharedFields[row.ItemID].Add(row);
          }

          DeleteVersionedFields(fieldId, itemId);

          return true;
        }
      }

      private bool MoveDataToUnversionedFromShared([NotNull] ID fieldId, [NotNull] ID itemId)
      {
        lock (Owner)
        {
          var languages = Owner.GetLanguages();

          DeleteUnversionedFields(fieldId, itemId);

          foreach (var language in languages)
          {
            IEnumerable<FieldsRow> rows;
            if (itemId != Data.ID.Null)
            {
              rows = Owner.SharedFields[itemId].Where(r => r.FieldID == fieldId);
            }
            else
            {
              rows =
                  Owner.SharedFields.Values.Select(list => list.Where(r => r.FieldID == fieldId))
                      .Aggregate((a, b) => a.Concat(b));
            }

            rows =
                rows.Where(
                    r =>
                        Owner.VersionedFields.Values.Select(
                            list => list.Any(v => v.ItemID == r.ItemID && v.Language == language.ToString()))
                            .Any(b => b));
            rows =
                rows.Select(
                    r =>
                        new FieldsRow
                        {
                          ItemID = r.ItemID,
                          FieldID = r.FieldID,
                          Language = language.ToString(),
                          Value = r.Value
                        });
            foreach (FieldsRow row in rows)
            {
              Owner.UnversionedFields[row.ItemID].Add(row);
            }
          }

          DeleteSharedFields(fieldId, itemId);
          DeleteVersionedFields(fieldId, itemId);

          return true;
        }
      }

      private bool MoveDataToUnversionedFromVersioned([NotNull] ID fieldId, [NotNull] ID itemId)
      {
        lock (Owner)
        {
          var languages = Owner.GetLanguages();

          DeleteSharedFields(fieldId, itemId);
          DeleteUnversionedFields(fieldId, itemId);

          foreach (var language in languages)
          {
            IEnumerable<FieldsRow> rows;

            if (itemId != Data.ID.Null)
            {
              rows =
                  Owner.VersionedFields[itemId].Where(
                      r => r.FieldID == fieldId && r.Language == language.ToString());
            }
            else
            {
              rows =
                  Owner.VersionedFields.Values.Select(
                      list => list.Where(r => r.FieldID == fieldId && r.Language == language.ToString()))
                      .Aggregate((a, b) => a.Concat(b));
              rows =
                  rows.GroupBy(r => r.ItemID.ToString())
                      .Select(g => g.First(r => r.Version == g.Max(r1 => r1.Version)));
            }

            rows =
                rows.Select(
                    r =>
                        new FieldsRow
                        {
                          ItemID = r.ItemID,
                          FieldID = r.FieldID,
                          Language = language.ToString(),
                          Value = r.Value
                        });

            foreach (var row in rows)
            {
              Owner.UnversionedFields[row.ItemID].Add(row);
            }
          }

          DeleteVersionedFields(fieldId, itemId);

          return true;
        }
      }

      private bool MoveDataToVersionedFromShared([NotNull] ID fieldId, [NotNull] ID itemId)
      {
        lock (Owner)
        {
          var languages = Owner.GetLanguages();
          var versions = Owner.GetVersions();

          DeleteUnversionedFields(fieldId, itemId);
          DeleteVersionedFields(fieldId, itemId);

          foreach (var language in languages)
          {
            foreach (var version in versions)
            {
              IEnumerable<FieldsRow> rows;
              if (itemId != Data.ID.Null)
              {
                rows = Owner.SharedFields[itemId].Where(r => r.FieldID == fieldId);
              }
              else
              {
                rows =
                    Owner.SharedFields.Values.Select(list => list.Where(r => r.FieldID == fieldId))
                        .Aggregate((a, b) => a.Concat(b));
              }

              rows =
                  rows.Where(
                      r =>
                          Owner.VersionedFields.Values.Select(
                              list =>
                                  list.Any(
                                      v =>
                                          v.ItemID == r.ItemID && v.Language == language.ToString()
                                          && v.Version == version.ToInt32())).Any(b => b));
              rows =
                  rows.Select(
                      r =>
                          new FieldsRow
                          {
                            ItemID = r.ItemID,
                            Language = language.ToString(),
                            Version = version.ToInt32(),
                            FieldID = r.FieldID,
                            Value = r.Value
                          });

              foreach (var row in rows)
              {
                Owner.VersionedFields[row.ItemID].Add(row);
              }
            }
          }

          DeleteSharedFields(fieldId, itemId);

          return true;
        }
      }

      private bool MoveDataToVersionedFromUnversioned([NotNull] ID fieldId, [NotNull] ID itemId)
      {
        lock (Owner)
        {
          var languages = Owner.GetLanguages();
          var versions = Owner.GetVersions();

          DeleteVersionedFields(fieldId, itemId);
          foreach (var language in languages)
          {
            foreach (var version in versions)
            {
              IEnumerable<FieldsRow> rows;
              if (itemId != Data.ID.Null)
              {
                rows = Owner.UnversionedFields[itemId].Where(r => r.FieldID == fieldId);
              }
              else
              {
                rows =
                    Owner.UnversionedFields.Values.Select(
                        list =>
                            list.Where(r => r.FieldID == fieldId && r.Language == language.ToString()))
                        .Aggregate((a, b) => a.Concat(b));
              }

              rows =
                  rows.Where(
                      r =>
                          Owner.VersionedFields.Values.Select(
                              list =>
                                  list.Any(
                                      v =>
                                          v.ItemID == r.ItemID && v.Language == language.ToString()
                                          && v.Version == version.ToInt32())).Any(b => b));
              rows =
                  rows.Select(
                      r =>
                          new FieldsRow
                          {
                            ItemID = r.ItemID,
                            Language = r.Language,
                            Version = version.ToInt32(),
                            FieldID = r.FieldID,
                            Value = r.Value
                          });

              foreach (FieldsRow row in rows)
              {
                Owner.VersionedFields[row.ItemID].Add(row);
              }
            }
          }

          DeleteUnversionedFields(fieldId, itemId);

          return true;
        }
      }

      #endregion
    }
  }
}