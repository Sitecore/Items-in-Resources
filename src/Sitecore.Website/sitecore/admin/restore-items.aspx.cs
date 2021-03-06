﻿using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Sitecore.sitecore.admin
{
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.DataProviders;
  using Sitecore.Extensions.Database;
  using Sitecore.SecurityModel;

  public partial class RestoreItemsPage : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (IsPostBack)
      {
        return;
      }

      Refresh(sender, e);
    }

    protected void Refresh(object sender, EventArgs e)
    {
      ListBox.Items.Clear();
      using (new SecurityDisabler())
      {
        foreach (var database in Factory.GetDatabases())
        {
          if (string.IsNullOrEmpty(database?.ConnectionStringName))
          {
            continue;
          }

          var listItems = database
            .GetDataProviders()
            .First()
            .GetChildIDs(new ItemDefinition(Data.ID.Undefined, "", Data.ID.Undefined, Data.ID.Undefined), new CallContext(database.DataManager, 1))
            .Cast<ID>()
            .Select(x => database.GetItem(x))
            .Select(x =>
              new ListItem(
                $"{x.Name} ({x.Uri})",
                x.Uri.ToString()))
            .ToArray();

          ListBox.Items.AddRange(listItems);
        }
      }
    }

    protected void Restore(object sender, EventArgs e)
    {
      var selectedItem = ListBox.SelectedItem;
      if (selectedItem == null)
      {
        return;
      }

      var uriText = selectedItem.Value;
      var uri = ItemUri.Parse(uriText);
      var db = Factory.GetDatabase(uri.DatabaseName);
      var dataProvider = db.GetDataProviders()
        .OfType<CompositeDataProvider>()
        .First();

      var headProvider = dataProvider
        .HeadProvider;

      var itemDefinition = new ItemDefinition(uri.ItemID, string.Empty, Data.ID.Undefined, Data.ID.Undefined);
      var callContext = new CallContext(db.DataManager, 1);
      headProvider.DeleteItem(itemDefinition, callContext);
      db.RemoveFromCaches(uri.ItemID);

      var parentId = dataProvider.GetParentID(itemDefinition, callContext);
      db.RemoveFromCaches(parentId);

      ListBox.Items.Remove(selectedItem);
    }
  }
}