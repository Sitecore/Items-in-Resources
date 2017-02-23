namespace Sitecore
{
  using System;
  using Sitecore.Data.DataProviders;

  /// <summary>
  /// The only purpose of this class is to prevent R# from removing corresponding assemblies via "Remove Unused References"
  /// </summary>
  internal static class Project
  {
    [UsedImplicitly]
    private static readonly Type[] Dependencies =
    {
      typeof(AbstractDataProvider),
      typeof(CompositeDataProvider),
      typeof(MemoryDataProvider),
      typeof(ProtobufDataProvider),
      typeof(SqlServerDataProvider),
    };
  }
}