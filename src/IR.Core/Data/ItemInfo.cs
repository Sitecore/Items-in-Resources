namespace IR.Data
{
  using System;
  using ProtoBuf;

  [ProtoContract]
  public sealed class ItemInfo
  {
    [ProtoMember(1)]
    public Guid ID;   

    [ProtoMember(2)]
    public string Name;

    [ProtoMember(3)]
    public Guid ParentID { get; set; }

    [ProtoMember(4)]
    public Guid TemplateID;

    //[ProtoMember(5)]
    //public Guid BranchID;   
  }
}