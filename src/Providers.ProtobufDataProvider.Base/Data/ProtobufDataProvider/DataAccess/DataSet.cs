namespace Sitecore.Data.ProtobufDataProvider.DataAccess
{
  using System.IO;
  using ProtoBuf;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ItemsDataSet
  {
    [NotNull]
    public readonly ChildrenDataSet Children;

    [NotNull]
    public readonly ItemDataRecordSet ItemDataRecord;

    [NotNull]
    public readonly LanguageDataSet LanguageData;

    [NotNull]
    public readonly SharedDataSet SharedData;

    public ItemsDataSet(FileInfo definitions, FileInfo sharedData, FileInfo languageData) : this(OpenDefinitions(definitions), OpenSharedData(sharedData), OpenLanguageData(languageData))
    {
    }

    public ItemsDataSet(Stream definitions, Stream sharedData, Stream languageData)
    {
      using (definitions)
      {
        var info = Serializer.Deserialize<ItemDataRecordSet>(definitions);
        Assert.IsNotNull(info, nameof(info));

        ItemDataRecord = info;
      }

      Children = new ChildrenDataSet(ItemDataRecord);

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

    public static ItemsDataSet OpenRead(Stream definitions, Stream sharedData, Stream langData)
    {
      return new ItemsDataSet(definitions, sharedData, langData);
    }
  }
}