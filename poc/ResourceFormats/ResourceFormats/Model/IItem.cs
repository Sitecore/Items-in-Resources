using System;
using System.Collections.Generic;

namespace ResourceFormats.Model
{
  public interface IReadOnlyItem
  {
    Guid ID { get; }
    Guid ParentID { get; }
    Guid TemplateID { get; }
    string Name { get; }
    
    IReadOnlyCollection<string> Languages { get; }

    IReadOnlyCollection<int> GetVersions(string language);

    IReadOnlyDictionary<Guid, string> GetFields(string language, int version);
  }
}