namespace IR.Tests
{
  using System.Linq;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class StringExtensionsTests
  {
    [TestMethod]
    public void SplitTest1()
    {
      var result = "ab||bc||cd".Split("||").ToArray();
      
      Assert.AreEqual(3, result.Length);
      Assert.AreEqual("ab", result[0]);
      Assert.AreEqual("bc", result[1]);
      Assert.AreEqual("cd", result[2]);
    }

    [TestMethod]
    public void SplitTest2()
    {
      var result = "||ab||bc||cd||".Split("||").ToArray();

      Assert.AreEqual(5, result.Length);
      Assert.AreEqual("", result[0]);
      Assert.AreEqual("ab", result[1]);
      Assert.AreEqual("bc", result[2]);
      Assert.AreEqual("cd", result[3]);
      Assert.AreEqual("", result[4]);
    }

    [TestMethod]
    public void SplitTest3()
    {
      var result = "||ab||bc||cd||".Split("||", false).ToArray();

      Assert.AreEqual(3, result.Length);
      Assert.AreEqual("ab", result[0]);
      Assert.AreEqual("bc", result[1]);
      Assert.AreEqual("cd", result[2]);
    }
  }
}
