namespace Sitecore.Data.ProtobufDataProvider.DataFormat
{
  using ProtoBuf;

  [ProtoContract]
  public class ItemsData
  {
    [ProtoMember(1)]
    public ItemDefinitions Definitions { get; set; }

    [ProtoMember(2)]
    public ItemsSharedData SharedData { get; set; }

    [ProtoMember(3)]
    public ItemsLanguagesData LanguageData { get; set; }
  }
}