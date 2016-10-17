namespace Sitecore.Tests
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using ProtoBuf;
  using Sitecore.Data;

  [TestClass]
  public class ProtobufTests
  {
    [TestMethod]
    public void SerializeDeserializeObjTest()
    {
      // arrange
      var stream = new MemoryStream();
      var source = new ItemInfo
      {
        Name = "Test",
        TemplateID = new Guid()
      };

      // act
      Serializer.Serialize(stream, source);
      stream.Seek(0, SeekOrigin.Begin);
      var copy = Serializer.Deserialize<ItemInfo>(stream);

      // assert
      Assert.AreNotEqual(source, copy);
      Assert.AreEqual(source.Name, copy.Name);
      Assert.AreEqual(source.TemplateID, copy.TemplateID);
    }

    [TestMethod]
    public void SerializeDeserializeEmptyDictionaryTest()
    {
      // arrange
      var stream = new MemoryStream();
      var source = new Dictionary<Guid, string>();

      var wrapIn = new Wrap1
      {
        Dict = source
      };

      // act
      Serializer.Serialize(stream, wrapIn);
      stream.Seek(0, SeekOrigin.Begin);
      var wrapOut = Serializer.Deserialize<Wrap1>(stream);
      var copy = wrapOut.Dict;

      // assert      
      Assert.IsNull(copy);
    }

    [TestMethod]
    public void SerializeDeserializeDictionaryTest()
    {
      // arrange
      var stream = new MemoryStream();
      var source = new Dictionary<Guid, string>
      {
        {Guid.NewGuid(), "Test1"},
        {Guid.NewGuid(), "Test2"}
      };

      var wrapIn = new Wrap1
      {
        Dict = source
      };

      // act
      Serializer.Serialize(stream, wrapIn);
      stream.Seek(0, SeekOrigin.Begin);
      var wrapOut = Serializer.Deserialize<Wrap1>(stream);
      var copy = wrapOut.Dict;

      // assert      
      Assert.AreEqual(2, copy.Count);
      Assert.AreEqual(source.First().Key, copy.First().Key);
      Assert.AreEqual(source.First().Value, copy.First().Value);
      Assert.AreEqual(source.Last().Key, copy.Last().Key);
      Assert.AreEqual(source.Last().Value, copy.Last().Value);
    }

    [TestMethod]
    public void SerializeDeserializeDictionaryVolumeTest()
    {
      // arrange
      var stream = new MemoryStream();
      var source = new Dictionary<Guid, KeyValuePair<string, Guid>>();
      var size = 100;
      for (var i = 0; i < size; ++i)
      {
        source.Add(Guid.NewGuid(), new KeyValuePair<string, Guid>("12345678901234567890123", Guid.NewGuid())); // ID, 23-char name, TemplateID = 69B
      }

      var wrapIn = new Wrap2
      {
        Dict = source
      };

      // act
      Serializer.Serialize(stream, wrapIn);
      Console.WriteLine(stream.Length);
    }

    [ProtoContract]
    public class Wrap1
    {
      [ProtoMember(1)]
      public Dictionary<Guid, string> Dict;
    }

    [ProtoContract]
    public class Wrap2
    {
      [ProtoMember(1)]
      public Dictionary<Guid, KeyValuePair<string, Guid>> Dict;
    }
  }
}