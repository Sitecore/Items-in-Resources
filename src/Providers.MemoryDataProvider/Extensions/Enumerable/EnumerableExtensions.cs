namespace Sitecore.Extensions.Enumerable
{
  using System;
  using System.Collections.Generic;

  internal static class EnumerableExtensions
  {
    [NotNull]
    public static IEnumerable<T> Apply<T>([NotNull] this IEnumerable<T> sequence, [NotNull] Action<T> action)
    {
      foreach (var i in sequence)
      {
        action(i);
      }

      return sequence;
    }
  }
}