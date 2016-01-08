namespace IR.Convert
{
  using ProtoBuf;
  using Sitecore.Diagnostics.Base.Annotations;

  [ProtoContract]
  public class ProtobufWrap<T>
  {
    [ProtoMember(1)]
    public T Data
    {
      [UsedImplicitly]
      get;

      set;
    }
  }

  public static class ProtobufWrap
  {
    public static ProtobufWrap<T> Create<T>(T data)
    {
      return new ProtobufWrap<T>
      {
        Data = data
      };
    }
  }
}