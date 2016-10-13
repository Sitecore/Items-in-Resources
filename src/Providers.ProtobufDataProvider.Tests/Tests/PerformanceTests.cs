namespace Sitecore.Tests
{
  using System;
  using System.Diagnostics;
  using Sitecore.Data.DataAccess;
  using Sitecore.Resources;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class PerformanceTests
  {
    [TestMethod]
    public void TestRead()
    {
      // arrange
      var db = "core";

      // act   
      {
        var definitions = Resource.GetStream($"{db}.definitions.dat");
        var sharedData = Resource.GetStream($"{db}.data.shared.dat");
        var langData = Resource.GetStream($"{db}.data.lang.dat");

        GC.Collect();

        var mem = Process.GetCurrentProcess().PrivateMemorySize64;
        var dataSet = new DataSet(definitions, sharedData, langData);
        Console.WriteLine((Process.GetCurrentProcess().PrivateMemorySize64 - mem) / 1024 / 1024 + "MB");

        // assert
        Assert.IsNotNull(dataSet);
        Assert.AreEqual(18348, dataSet.ItemInfo.Count); // SELECT COUNT(*) AS B FROM [dbo].[Items] // 4627 for master
        Assert.AreEqual(4440, dataSet.Children.Count); // SELECT COUNT(A.B) FROM (SELECT [ParentID] AS B FROM [dbo].[Items] GROUP BY [ParentID]) AS A // 1388 for master
      }

      GC.Collect();
           
      // act and assert performance     
      var sw = new Stopwatch();
      var repeat = 1;
      for (var i = 0; i < repeat; ++i)
      {
        var definitions = Resource.GetStream($"{db}.definitions.dat");
        var sharedData = Resource.GetStream($"{db}.data.shared.dat");
        var langData = Resource.GetStream($"{db}.data.lang.dat");

        sw.Start();
        DataSet.OpenRead(definitions, sharedData, langData);
        sw.Stop();
      }
             
      Console.WriteLine(new TimeSpan(sw.Elapsed.Ticks / repeat));
    }
  }
}
