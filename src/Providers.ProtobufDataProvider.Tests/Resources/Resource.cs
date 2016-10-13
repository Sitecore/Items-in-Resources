namespace Sitecore.Resources
{
  using System;
  using System.IO;
  using System.Reflection;

  public static class Resource
  {
    private static readonly Type Type = typeof(Resource);
    private static readonly Assembly Assembly = Type.Assembly;

    public static Stream GetStream(string name)
    {
      var file = new FileInfo(Path.GetTempFileName());

      // save embedded resource into temp file (for accurate benchmarking)
      SaveEmbeddedResourceToFile(name, file);

      // open file stream
      return file.OpenRead();
    }

    private static void SaveEmbeddedResourceToFile(string name, FileInfo file)
    {
      using (var writer = file.OpenWrite())
      {
        var size = 1024;
        var buffer = new byte[size];
        using (var reader = Assembly.GetManifestResourceStream($"{Type.Namespace}.{name}"))
        {
          int len;
          while ((len = reader.Read(buffer, 0, size)) > 0)
          {
            writer.Write(buffer, 0, len);
          }
        }
      }
    }
  }
}
