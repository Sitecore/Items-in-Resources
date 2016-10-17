namespace Sitecore.Extensions.Enumerable
{
  using System;
  using System.Collections.Generic;

  public static partial class EnumerableExtensions
  {
    public static int FirstPositive<T>(this IEnumerable<T> enumerable, Func<T, int> func, int otherwise) 
    {                 
      foreach (var obj in enumerable)
      {
        var result = func(obj);
        if (result >= 0)
        {
          return result;
        }
      }

      return otherwise;
    }
  }
}