namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Sitecore.Collections;
  using Sitecore.Data.Items;
  using Sitecore.Data.Templates;
  using Sitecore.Eventing;
  using Sitecore.Globalization;      
  using Sitecore.Publishing.Pipelines.Publish;
  using Sitecore.Workflows;

  public abstract partial class ReadOnlyDataProvider
  {
    private const string MustNotBeCalled = "This method must not be called";

    private const string UseAnotherOverload = "This method must not be called";

    [Obsolete(MustNotBeCalled)]
    public sealed override EventQueue GetEventQueue()
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, DateTime created, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override int AddVersion(ItemDefinition itemDefinition, VersionUri baseVersion, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool BlobStreamExists(Guid blobId, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override Stream GetBlobStream(Guid blobId, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool SaveItem(ItemDefinition itemDefinition, ItemChanges changes, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool SetBlobStream(Stream stream, Guid blobId, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool DeleteItem(ItemDefinition itemDefinition, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool RemoveBlobStream(Guid blobId, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool RemoveVersion(ItemDefinition itemDefinition, VersionUri version, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool RemoveVersions(ItemDefinition itemDefinition, Language language, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool RemoveVersions(ItemDefinition itemDefinition, Language language, bool removeSharedData, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool CopyItem(ItemDefinition source, ItemDefinition destination, string copyName, ID copyID, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool MoveItem(ItemDefinition itemDefinition, ItemDefinition destination, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override void RemoveLanguageData(Language language, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override void RenameLanguageData(string fromLanguage, string toLanguage, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool ChangeFieldSharing(TemplateField fieldDefinition, TemplateFieldSharing sharing, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool ChangeTemplate(ItemDefinition itemDefinition, TemplateChangeList changes, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool AddToPublishQueue(ID itemID, string action, DateTime date, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool AddToPublishQueue(ID itemID, string action, DateTime date, string language, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool CleanupPublishQueue(DateTime to, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override IDList GetPublishQueue(DateTime @from, DateTime to, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override List<PublishQueueEntry> GetPublishQueueEntries(DateTime @from, DateTime to, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override DataUri[] GetItemsInWorkflowState(WorkflowInfo info, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override WorkflowInfo GetWorkflowInfo(ItemDefinition item, VersionUri version, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool SetWorkflowInfo(ItemDefinition item, VersionUri version, WorkflowInfo info, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool RemoveProperty(string name, bool isPrefix, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool SetProperty(string name, string value, CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(MustNotBeCalled)]
    public sealed override bool CleanupDatabase(CallContext context)
    {
      throw new NotSupportedException(MustNotBeCalled);
    }

    [Obsolete(UseAnotherOverload)]
    public sealed override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
    {
      throw new NotSupportedException(UseAnotherOverload);
    }

    [Obsolete(UseAnotherOverload)]
    public sealed override FieldList GetItemFields(ItemDefinition itemDefinition, VersionUri versionUri, CallContext context)
    {
      throw new NotSupportedException(UseAnotherOverload);
    }

    [Obsolete(UseAnotherOverload)]
    public sealed override VersionUriList GetItemVersions(ItemDefinition itemDefinition, CallContext context)
    {
      throw new NotSupportedException(UseAnotherOverload);
    }

    [Obsolete(UseAnotherOverload)]
    public sealed override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
    {
      throw new NotSupportedException(UseAnotherOverload);
    }

    [Obsolete(UseAnotherOverload)]
    public sealed override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
    {
      throw new NotSupportedException(UseAnotherOverload);
    }

    [Obsolete(UseAnotherOverload)]
    public sealed override LanguageCollection GetLanguages(CallContext context)
    {
      throw new NotSupportedException(UseAnotherOverload);
    }

    [Obsolete(UseAnotherOverload)]
    public sealed override IdCollection GetTemplateItemIds(CallContext context)
    {
      throw new NotSupportedException(UseAnotherOverload);
    }

    [Obsolete(UseAnotherOverload)]
    public sealed override bool HasChildren(ItemDefinition itemDefinition, CallContext context)
    {
      throw new NotSupportedException(UseAnotherOverload);
    }

    [Obsolete("User SelectIDs instead.")]
    public sealed override ID SelectSingleID(string query, CallContext context)
    {
      throw new NotSupportedException("User SelectIDs instead.");
    }

    [Obsolete(UseAnotherOverload)]
    public sealed override IDList SelectIDs(string query, CallContext context)
    {
      throw new NotSupportedException(UseAnotherOverload);
    }

    [Obsolete(UseAnotherOverload)]
    public sealed override ID ResolvePath(string itemPath, CallContext context)
    {
      throw new NotSupportedException(UseAnotherOverload);
    }
  }
}
