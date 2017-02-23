using Thread = System.Threading.Thread;

namespace Sitecore.Shell.Framework.Commands.ContentEditor
{                           
  using global::System.Linq; // R# doesn't like without global
  using Sitecore.Data.DataProviders;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Security.Accounts;
  using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;

  [UsedImplicitly]
  public class RemoveItemSqlData : Command
  {
    public override CommandState QueryState(CommandContext context)
    {
      var user = Context.User;
      var allowed = 
        user.IsAdministrator || 
        user.IsInRole(Role.FromName("sitecore\\Sitecore Client Developing")) ||
        false;

      return allowed ? CommandState.Enabled : CommandState.Hidden;
    }

    public override void Execute(CommandContext context)
    {
      var item = context.Items.First();
      ProgressBox.Execute(
        "DeleteSqlData", 
        Translate.Text("Delete"), 
        GetIcon(context, string.Empty), 
        Process, 
        $"item:load(id={item.ID})", 
        item, context);
    }

    private void Process(params object[] parameters)
    {
      var item = parameters[0] as Item;
      if (item == null)
      {
        return;
      }

      Process(item);
    }

    protected virtual void Process(Item item)
    {
      var dataProvider = item.Database
        .GetDataProviders()
        .OfType<CompositeDataProvider>()
        .First();

      if (dataProvider.CanBeRemovedFromHead(item))
      {
        Log.Audit($"Deleting SQL data of completely default item: {item.Name}, Uri: {item.Uri}", this);

        dataProvider.HeadProviderEx.RemoveItemData(item);
      }
    }   
  }
}