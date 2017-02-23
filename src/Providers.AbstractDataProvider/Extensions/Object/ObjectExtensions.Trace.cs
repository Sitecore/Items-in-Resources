namespace Sitecore.Extensions.Object
{
  using System.Collections;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.DataProviders;
  using Sitecore.Diagnostics;

  public static class ObjectExtensions
  {                          
    public static void Trace<T>([CanBeNull, UsedImplicitly] this T obj, [CanBeNull] object result, [CanBeNull] Stopwatch timer, [NotNull] params object[] arguments)
    {
      Assert.ArgumentNotNull(arguments, nameof(arguments));

      timer?.Stop();

      var st = new StackTrace();
      var sf = st.GetFrame(1);
      Assert.IsNotNull(sf, nameof(sf));

      var type = typeof(T).Name;
      var method = sf.GetMethod().Name;
      var argumentsText = string.Join(", ", arguments.Select(Print));
      var resultText = Print(result);
      var message = $"Tracing {type}.{method} call\r\nArguments: ({argumentsText})\r\nResult: {resultText}\r\nTime spent: {timer?.Elapsed.TotalMilliseconds.ToString() ?? "unknown "}ms";

      Log.Info(message, typeof(ObjectExtensions));
    }

    [NotNull]
    private static string Print(object obj)
    {
      if (obj == null)
      {
        return "null";
      }

      var text = obj as string;
      if (text != null)
      {
        return PrintValue(text);
      }

      var list = obj as IDList;
      if (list != null)
      {
        return PrintArray(list);
      }

      var enumerable = obj as IEnumerable;
      if (enumerable != null)
      {
        return PrintArray(enumerable);
      }

      var itemDefinition = obj as ItemDefinition;
      if (itemDefinition != null)
      {
        var objId = itemDefinition.ID;
        Assert.IsNotNull(objId, nameof(objId));

        obj = objId;
      }

      var callContext = obj as CallContext;
      if (callContext != null)
      {
        var objDb = callContext.DataManager?.Database;
        Assert.IsNotNull(objDb, nameof(objDb));

        obj = objDb;
      }              

      var stream = obj as Stream;
      if (stream != null)
      {
        var length = stream.Length;
        Assert.IsNotNull(length, nameof(length));

        obj = $"Stream, length: {length}";
      }

      var id = obj as ID;
      if (id != (ID)null)
      {
        return PrintValue(id);
      }

      return PrintValue(obj);
    }

    [NotNull]
    private static string PrintValue([NotNull] object obj)
    {
      Assert.ArgumentNotNull(obj, nameof(obj));

      return "\"" + obj + "\"";
    }

    [NotNull]
    private static string PrintArray([NotNull] IEnumerable list)
    {
      Assert.ArgumentNotNull(list, nameof(list));

      return "[" + string.Join(", ", list.Cast<object>().Select(Print).ToArray()) + "]";
    }
  }
}
