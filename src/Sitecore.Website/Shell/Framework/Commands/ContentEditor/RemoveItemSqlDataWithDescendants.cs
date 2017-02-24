namespace Sitecore.Shell.Framework.Commands.ContentEditor
{
  [UsedImplicitly]
  public class RemoveItemSqlDataWithDescendants : RemoveItemSqlData
  {
    protected override bool Recurse => true;
  }
}