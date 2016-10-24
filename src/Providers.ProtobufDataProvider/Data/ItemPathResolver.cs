namespace Sitecore.Data
{
  using System;
  using Sitecore.Data.DataAccess;

  public static class ItemPathResolver
  {
    /// <summary>
    ///   Try to find item ID by path
    /// </summary>
    /// <param name="itemPath"></param>
    /// <param name="childrenDataSet"></param>
    /// <param name="id">If item is successfully found equals to the item ID, otherwise the closest existing ancestor ID</param>
    /// <returns></returns>
    public static bool TryResolvePath(string itemPath, ChildrenDataSet childrenDataSet, out Guid id)
    {
      if (string.IsNullOrWhiteSpace(itemPath))
      {
        throw new ArgumentNullException(nameof(itemPath));
      }

      if (childrenDataSet == null)
      {
        throw new ArgumentNullException(nameof(childrenDataSet));
      }

      if (!itemPath.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
      {
        id = Guid.Empty;
        return false;
      }

      itemPath = itemPath.TrimEnd('/');
      if (itemPath.Length == "/sitecore".Length)
      {
        id = ItemIDs.RootItemID;
        return childrenDataSet.ContainsKey(ItemIDs.RootItemID);
      }

      if (itemPath["/sitecore".Length] != '/')
      {
        throw new ArgumentException($"itemPath must start with /sitecore/ (actual: {itemPath})");
      }

      id = ItemIDs.RootItemID;
      var substring = itemPath.Substring("/sitecore/".Length);
      var arr = substring.Split('/');

      return ResolvePath(childrenDataSet, ref id, arr, itemPath, 0);
    }

    private static bool ResolvePath(ChildrenDataSet childrenDataSet, ref Guid id, string[] arr, string path, int shift)
    {
      if (shift >= arr.Length)
      {
        return true;
      }

      var word = arr[shift];
      if (string.IsNullOrEmpty(word))
      {
        throw new ArgumentException($"itemPath must not contain double-slashes // (actual: {path})");
      }

      ItemInfo[] children;
      if (!childrenDataSet.TryGetValue(id, out children) || (children == null))
      {
        return false;
      }

      foreach (var child in children)
      {
        if ((child?.Name == null) || !child.Name.Equals(word, StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        var childId = child.ID;
        if (!ResolvePath(childrenDataSet, ref childId, arr, path, shift + 1))
        {
          continue;
        }

        id = childId;
        return true;
      }

      return false;
    }
  }
}