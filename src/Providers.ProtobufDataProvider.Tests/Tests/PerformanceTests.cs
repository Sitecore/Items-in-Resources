namespace Sitecore.Tests
{
  using System;
  using System.Diagnostics;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Sitecore.Data.ProtobufDataProvider.DataFormat;
  using Sitecore.Resources;

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
        
        GC.Collect();

        var mem = Process.GetCurrentProcess().PrivateMemorySize64;
        var dataSet = new ItemsDataSet(definitions);
        Console.WriteLine((Process.GetCurrentProcess().PrivateMemorySize64 - mem) / 1024 / 1024 + "MB");

        // assert
        Assert.IsNotNull(dataSet);
        Assert.AreEqual(22418, dataSet.Definitions.Count); // SELECT COUNT(*) AS B FROM [dbo].[Items] // 22418 for core, 4633 for master
        Assert.AreEqual(4450, dataSet.Children.Count); // SELECT COUNT(A.B) FROM (SELECT [ParentID] AS B FROM [dbo].[Items] GROUP BY [ParentID]) AS A // 4450 for core, 1389 for master
      }

      GC.Collect();

      // act and assert performance     
      var sw = new Stopwatch();
      var repeat = 1;
      for (var i = 0; i < repeat; ++i)
      {
        var definitions = Resource.GetStream($"{db}.definitions.dat");
        
        sw.Start();
        ItemsDataSet.OpenRead(definitions);
        sw.Stop();
      }

      Console.WriteLine(new TimeSpan(sw.Elapsed.Ticks / repeat));
    }
  }
}