namespace Sitecore.Extensions.Enumerable
{
  using System;
  using System.Collections.Generic;

  public static partial class EnumerableExtensions
  {
    public static TR FirstNotNull<T, TR>(this IEnumerable<T> enumerable, Func<T, TR> func) where TR : class
    {
      foreach (var obj in enumerable)
      {
        var result = func(obj);
        if (result != null)
        {
          return result;
        }
      }

      return null;
    }
  }
}