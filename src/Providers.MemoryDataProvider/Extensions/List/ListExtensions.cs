namespace Sitecore.Extensions.List
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Extensions.Enumerable;

  internal static class ListExtensions
  {
    public static List<T> Add<T>([NotNull] this List<T> list, [NotNull] IEnumerable<T> collection)
    {
      foreach (var i in collection)
      {
        list.Add(i);
      }

      return list;
    }

    public static List<T> Delete<T>([NotNull] this List<T> list, [NotNull] Func<T, bool> predicate)
    {
      list.Where(predicate).ToList().Apply(r => list.Remove(r));
      return list;
    }
  }
}