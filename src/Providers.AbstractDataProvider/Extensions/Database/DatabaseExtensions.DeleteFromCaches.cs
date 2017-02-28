namespace Sitecore.Extensions.Database
{
  using Sitecore.Caching;
  using Sitecore.Data;

  public static class DatabaseExtensions
  {
    public static void RemoveFromCaches(this Database db, ID id)
    {
      CacheManager.FindCacheByName<ID>($"SqlDataProvider - Prefetch data({db.Name})").Remove(id);
      db.Caches.DataCache.RemoveItemInformation(id);
      db.Caches.ItemCache.RemoveItem(id);
    }
  }
}
