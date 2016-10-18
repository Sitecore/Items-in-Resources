namespace Sitecore.Extensions.DataProvider
{
  using Sitecore.Data;
  using Sitecore.Data.DataProviders;
  using Sitecore.Diagnostics;

  public static partial class DataProviderExtensions
  {
    [NotNull]
    public static void SetDatabase([NotNull] this DataProvider dataProvider, [NotNull] Database database)
    {
      Assert.ArgumentNotNull(dataProvider, nameof(dataProvider));
      Assert.ArgumentNotNull(database, nameof(database));

      typeof(DataProvider)
        .GetProperty("Database")
        .SetValue(dataProvider, database);
    }
  }
}
