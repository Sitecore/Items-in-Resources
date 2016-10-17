namespace Sitecore.Data.Generic
{
  using System;
  using System.Collections.Generic;
  using System.Runtime.Serialization;

  [Serializable]
  public class ListDictionary<T, K> : Dictionary<T, List<K>>
  {
    #region Public properties

    /// <summary>
    ///   Returns list by the key, creating key if it doesn't exist.
    /// </summary>
    public new List<K> this[T key]
    {
      get
      {
        List<K> result;
        if (TryGetValue(key, out result))
        {
          return result;
        }

        result = new List<K>();
        this[key] = result;

        return result;
      }

      set
      {
        base[key] = value;
      }
    }

    #endregion

    #region Constructors

    public ListDictionary()
    {
    }

    protected ListDictionary([NotNull] SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    #endregion
  }
}