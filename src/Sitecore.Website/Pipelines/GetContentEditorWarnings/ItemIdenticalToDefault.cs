namespace Sitecore.Pipelines.GetContentEditorWarnings
{
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Data.DataProviders;
  using Sitecore.Diagnostics;
  using Sitecore.Security.Accounts;

  public class ItemIdenticalToDefault
  {
    [UsedImplicitly]
    public void Process([NotNull] GetContentEditorWarningsArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));
              
      var user = Context.User;
      var allowed =
        user.IsAdministrator ||
        user.IsInRole(Role.FromName("sitecore\\Sitecore Client Developing")) ||
        false;

      if (!allowed)
      {
        return;
      }

      var item = args.Item;
      var dataProvider = item.Database
        .GetDataProviders()
        .OfType<CompositeDataProvider>()
        .First();

      if (!dataProvider.CanBeRemovedFromHead(item))
      {
        return;
      }
      
      var newWarning = args.Add();
      newWarning.Title = "Item is completely default";
      newWarning.Text = "Current item is stored in SQL database, but it is completely default and can be removed from SQL database without any negative effects on the system.";
      newWarning.Options.Add(new Pair<string, string>("Remove item SQL data", "item:removesqldata"));
      newWarning.Options.Add(new Pair<string, string>("Remove item SQL data with all descendant items in this condition", "item:removesqldatawithdescendants"));

      foreach (var warning in args.Warnings)
      {
        if (warning.IsExclusive)
        {
          warning.IsExclusive = false;
        }
      }
    }
  }
}