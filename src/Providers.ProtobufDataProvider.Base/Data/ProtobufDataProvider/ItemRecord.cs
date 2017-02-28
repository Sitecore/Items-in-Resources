namespace Sitecore.Data.ProtobufDataProvider
{
  using System;
  using ProtoBuf;

  [ProtoContract]
  public sealed class ItemRecord
  {
    [ProtoMember(1)]
    public Guid ID;   

    [ProtoMember(2)]
    public string Name;

    [ProtoMember(3)]
    public Guid ParentID;

    [ProtoMember(4)]
    public Guid TemplateID;

    //[ProtoMember(5)]
    //public Guid BranchID;   
  }
}