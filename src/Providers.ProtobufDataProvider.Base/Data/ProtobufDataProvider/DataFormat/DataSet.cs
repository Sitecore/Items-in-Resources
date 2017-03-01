namespace Sitecore.Data.ProtobufDataProvider.DataFormat
{
  using System.IO;
  using ProtoBuf;
  using Sitecore.Data.ProtobufDataProvider.DataAccess;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ItemsDataSet
  {
    [NotNull]
    public readonly ChildrenDataSet Children;

    [NotNull]
    public readonly ItemDefinitions Definitions;

    [NotNull]
    public readonly ItemsLanguagesData LanguageData;

    [NotNull]
    public readonly ItemsSharedData SharedData;

    public ItemsDataSet(FileInfo definitions) : this(OpenDefinitions(definitions))
    {
    }

    public ItemsDataSet(Stream stream)
    {
      using (stream)
      {
        var data = Serializer.Deserialize<ItemsData>(stream);
        Assert.IsNotNull(data, nameof(data));

        var definitions = data.Definitions;
        Assert.IsNotNull(definitions, nameof(definitions));

        var sharedData = data.SharedData;
        Assert.IsNotNull(sharedData, nameof(sharedData));

        var languageData = data.LanguageData;
        Assert.IsNotNull(languageData, nameof(languageData));

        Definitions = definitions;                           
        SharedData = sharedData;     
        LanguageData = languageData;
      }

      Children = new ChildrenDataSet(Definitions);   
    }

    private static Stream OpenDefinitions(FileInfo file)
    {
      return Open(file);
    }                              

    private static FileStream Open(FileInfo definitions)
    {
      return definitions.OpenRead();
    }

    public static ItemsDataSet OpenRead(Stream definitions)
    {
      return new ItemsDataSet(definitions);
    }
  }
}