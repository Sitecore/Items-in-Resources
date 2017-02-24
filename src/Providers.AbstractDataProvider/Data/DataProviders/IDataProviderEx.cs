using System.Collections.Generic;

namespace Sitecore.Data.DataProviders
{
  using Sitecore.Data.DataProviders.Sql; 

  public interface IDataProviderEx
  {
    IEnumerable<ID> GetChildIdsByName(string childName, ID parentId);

    void RemoveItemData(ID itemId);

    DataProviderTransaction OpenTransaction();
  }
}