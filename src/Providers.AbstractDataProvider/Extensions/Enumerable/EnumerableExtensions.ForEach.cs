namespace Sitecore.Extensions.Enumerable
{
  using System;
  using System.Collections.Generic;

  public static partial class EnumerableExtensions
  {
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) 
    {                 
      foreach (var obj in enumerable)
      {
        action(obj);
      }
    }
  }
}