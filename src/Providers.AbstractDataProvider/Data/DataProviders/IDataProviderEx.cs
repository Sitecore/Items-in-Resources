using System.Collections.Generic;

namespace Sitecore.Data.DataProviders
{
  using Sitecore.Data.Items;

  public interface IDataProviderEx
  {
    IEnumerable<ID> GetChildIdsByName(string childName, ID parentId);

    void RemoveItemData(Item item);
  }
}