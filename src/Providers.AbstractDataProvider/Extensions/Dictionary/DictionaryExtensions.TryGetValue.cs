namespace Sitecore.Extensions.Dictionary
{
  using System.Collections.Generic;

  public static class DictionaryExtensions
  {
    public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
      TValue value;
      return dictionary.TryGetValue(key, out value) ? value : default(TValue);
    }
  }
}
