namespace IR.Data
{
  using System;
  using System.Collections.Generic;
  using IR.Data.DataFormat;

  public static class Empty
  {
    public static readonly Dictionary<Guid, string> Fields = new Dictionary<Guid, string>();
    internal static readonly ItemLanguagesData Versions = new ItemLanguagesData();
  }
}