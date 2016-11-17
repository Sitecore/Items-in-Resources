using System.Collections.Generic;

namespace Sitecore.Data.DataProviders
{
  public interface IDataProviderEx
  {
    IEnumerable<ID> GetChildIdsByName(string childName, ID parentId);
  }
}