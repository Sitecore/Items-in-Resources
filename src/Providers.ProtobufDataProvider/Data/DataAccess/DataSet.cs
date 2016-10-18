namespace Sitecore.Data.DataAccess
{
  using System.IO;
  using ProtoBuf;
  using Sitecore.Diagnostics;

  public class DataSet
  {
    [NotNull]
    public readonly ChildrenDataSet Children;

    [NotNull]
    public readonly ItemInfoSet ItemInfo;

    [NotNull]
    public readonly LanguageDataSet LanguageData;

    [NotNull]
    public readonly SharedDataSet SharedData;

    public DataSet(FileInfo definitions, FileInfo sharedData, FileInfo languageData) : this(OpenDefinitions(definitions), OpenSharedData(sharedData), OpenLanguageData(languageData))
    {
    }

    public DataSet(Stream definitions, Stream sharedData, Stream languageData)
    {
      using (definitions)
      {
        var info = Serializer.Deserialize<ItemInfoSet>(definitions);
        Assert.IsNotNull(info, nameof(info));

        ItemInfo = info;
      }

      Children = new ChildrenDataSet(ItemInfo);

      using (sharedData)
      {
        SharedData = new SharedDataSet(sharedData);
      }

      using (languageData)
      {
        LanguageData = new LanguageDataSet(languageData);
      }
    }

    private static Stream OpenDefinitions(FileInfo file)
    {
      return Open(file);
    }

    private static Stream OpenSharedData(FileInfo file)
    {
      return Open(file);
    }

    private static Stream OpenLanguageData(FileInfo file)
    {
      return Open(file);
    }

    private static FileStream Open(FileInfo definitions)
    {
      return definitions.OpenRead();
    }

    public static DataSet OpenRead(Stream definitions, Stream sharedData, Stream langData)
    {
      return new DataSet(definitions, sharedData, langData);
    }
  }
}