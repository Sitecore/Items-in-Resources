namespace Sitecore.Tests
{
  using System;
  using Sitecore.Data;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Sitecore.Data.ProtobufDataProvider;
  using Sitecore.Data.ProtobufDataProvider.DataAccess;

  [TestClass]
  public class ItemPathResolverTests
  {
    [TestMethod]
    public void TesT1()
    {
      var root = new ItemRecord { ID = ItemIDs.RootItemID, Name = "Sitecore" };
      var namesCache = new ChildrenDataSet(new ItemDataRecordSet(root));

      // act
      Guid id;
      var result = ItemPathResolver.TryResolvePath("/home", namesCache, out id);

      Assert.IsFalse(result);       
      Assert.AreEqual(Guid.Empty, id);
    }

    [TestMethod]
    public void TesT2()
    {
      var root = new ItemRecord { ID = ItemIDs.RootItemID, Name = "Sitecore" };
      var namesCache = new ChildrenDataSet(new ItemDataRecordSet(root));

      // act
      Guid id;
      var result = ItemPathResolver.TryResolvePath("/sitecore", namesCache, out id);

      Assert.IsTrue(result);
      Assert.AreEqual(root.ID, id);
    }

    [TestMethod]
    public void TesT22()
    {
      var root = new ItemRecord { ID = ItemIDs.RootItemID, Name = "Sitecore" };
      var namesCache = new ChildrenDataSet(new ItemDataRecordSet(root));

      // act
      Guid id;
      var result = ItemPathResolver.TryResolvePath("/sitecore/", namesCache, out id);

      Assert.IsTrue(result);
      Assert.AreEqual(root.ID, id);
    }

    [TestMethod]
    public void TesT3()
    {
      var root = new ItemRecord { ID = ItemIDs.RootItemID, Name = "Sitecore" };
      var content = new ItemRecord { ID = Guid.NewGuid(), Name = "content", ParentID = root.ID };
      var namesCache = new ChildrenDataSet(new ItemDataRecordSet(root, content));

      // act
      Guid id;
      var result = ItemPathResolver.TryResolvePath("/sitecore/content", namesCache, out id);

      Assert.IsTrue(result);
      Assert.AreEqual(content.ID, id);
    }

    [TestMethod]
    public void TesT4()
    {
      var root = new ItemRecord { ID = ItemIDs.RootItemID, Name = "Sitecore" };
      var content = new ItemRecord { ID = Guid.NewGuid(), Name = "content", ParentID = root.ID };
      var namesCache = new ChildrenDataSet(new ItemDataRecordSet(root, content));

      // act
      Guid id;
      var result = ItemPathResolver.TryResolvePath("/sitecore/system", namesCache, out id);

      Assert.IsFalse(result);
      Assert.AreEqual(root.ID, id);
    }       

    [TestMethod]
    public void TesT5()
    {
      var root = new ItemRecord { ID = ItemIDs.RootItemID, Name = "Sitecore" };
      var content1 = new ItemRecord { ID = Guid.NewGuid(), Name = "content", ParentID = root.ID };
      var content2 = new ItemRecord { ID = Guid.NewGuid(), Name = "content", ParentID = root.ID };
      var home = new ItemRecord { ID = Guid.NewGuid(), Name = "Home", ParentID = content2.ID };
      var namesCache = new ChildrenDataSet(new ItemDataRecordSet(root, content1, content2, home));

      // act
      Guid id;
      var result = ItemPathResolver.TryResolvePath("/sitecore/content/home", namesCache, out id);

      Assert.IsTrue(result);
      Assert.AreEqual(home.ID, id);
    }    
  }
}
