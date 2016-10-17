namespace Sitecore
{
  using System;
  using System.Collections.Generic;

  public static class StringExtensions
  {
    public static IEnumerable<string> Split(this string that, string delimiter, bool allowEmptyOutput = true)
    {
      int pos;
      var prevPosPlusDelim = 0;
      while ((pos = that.IndexOf(delimiter, prevPosPlusDelim, StringComparison.Ordinal)) >= 0) // actually it must be >= prevPos, but who cares
      {
        var value = that.Substring(prevPosPlusDelim, pos - prevPosPlusDelim);
        if (allowEmptyOutput || !string.IsNullOrEmpty(value))
        {
          yield return value;
        }

        prevPosPlusDelim = pos + delimiter.Length;
        if (prevPosPlusDelim > that.Length)
        {
          yield break;
        }
      }

      if (allowEmptyOutput || prevPosPlusDelim < that.Length)
      {
        yield return that.Substring(prevPosPlusDelim);
      }
    }
  }
}
