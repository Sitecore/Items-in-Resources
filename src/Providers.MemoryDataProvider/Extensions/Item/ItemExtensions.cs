namespace Sitecore.Extensions.Item
{
  using Sitecore.Data;

  internal static class ItemExtensions
  {
    [NotNull]
    public static ItemDefinition Clone([NotNull] this ItemDefinition original, [NotNull] ID newid)
    {
      return new ItemDefinition(newid, original.Name, original.TemplateID, original.BranchId);
    }
  }
}