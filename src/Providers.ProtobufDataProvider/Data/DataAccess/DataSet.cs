namespace Sitecore.Data.DataAccess
{
  using System.IO;
  using ProtoBuf;

  public class DataSet
  {                                          
    public readonly ItemInfoSet ItemInfo;
                                             
    public readonly SharedDataSet SharedData;
                                             
    public readonly LanguageDataSet LanguageData;

    public readonly ChildrenDataSet Children;

    public DataSet(FileInfo definitions, FileInfo sharedData, FileInfo languageData) : this(OpenDefinitions(definitions), OpenSharedData(sharedData), OpenLanguageData(languageData))
    {      
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

    public DataSet(Stream definitions, Stream sharedData, Stream languageData)
    {
      using (definitions)
      {
        ItemInfo = Serializer.Deserialize<ItemInfoSet>(definitions);
      }

      Children = new ChildrenDataSet(ItemInfo);

      SharedData = new SharedDataSet(sharedData);
      LanguageData = new LanguageDataSet(languageData);
    }           

    public static DataSet OpenRead(Stream definitions, Stream sharedData, Stream langData)
    {
      return new DataSet(definitions, sharedData, langData);      
    }
  }
}