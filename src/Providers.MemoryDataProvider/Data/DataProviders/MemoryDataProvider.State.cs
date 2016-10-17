namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Data.Generic;

  public partial class MemoryDataProvider
  {  
    [Serializable]
    private class State
    {
      #region Fields
      public List<PublishQueueRow> _publishQueue = new List<PublishQueueRow>();
      public Dictionary<ID, ItemsRow> items = new Dictionary<ID, ItemsRow>();
      public object mutex = new object();
      public Dictionary<string, string> properties = new Dictionary<string, string>();
      public ListDictionary<ID, FieldsRow> sharedFields = new ListDictionary<ID, FieldsRow>();
      public ListDictionary<ID, FieldsRow> unversionedFields = new ListDictionary<ID, FieldsRow>();
      public ListDictionary<ID, FieldsRow> versionedFields = new ListDictionary<ID, FieldsRow>();
      public int version;

      #endregion
    }
  }
}
