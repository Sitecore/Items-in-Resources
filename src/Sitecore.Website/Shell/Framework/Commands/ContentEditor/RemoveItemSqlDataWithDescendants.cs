namespace Sitecore.Shell.Framework.Commands.ContentEditor
{
  using global::System.Linq;
  using Sitecore.Data.Items;

  [UsedImplicitly]
  public class RemoveItemSqlDataWithDescendants : RemoveItemSqlData
  {
    protected override void Process(Item item)
    {
      // save children
      var children = item.Children.Cast<Item>().ToArray();

      // process current item
      base.Process(item);

      // process children
      foreach (var child in children)
      {
        Process(child);
      }
    }
  }
}