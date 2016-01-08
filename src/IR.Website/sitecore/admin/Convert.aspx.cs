namespace IR.sitecore.admin
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using IR.Data;
  using IR.Data.DataFormat;
  using ProtoBuf;
  using Sitecore;
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.SecurityModel;

  public partial class Convert : System.Web.UI.Page
  {
    protected void ConvertItems(object sender, EventArgs e)
    {
      Log.Info("Converting SQL items to Items-in-Resources format", this);
      var dir = MainUtil.MapPath("/App_Data");
      if (!Directory.Exists(dir))
      {
        Directory.CreateDirectory(dir);
      }

      var databases = new[] { "core", "master", "web" };
      foreach (var databaseName in databases)
      {
        var definitionsFilePath = Path.Combine(dir, $"{databaseName}.definitions.dat");
        var sharedDataFilePath = Path.Combine(dir, $"{databaseName}.data.shared.dat");
        var languageDataFilePath = Path.Combine(dir, $"{databaseName}.data.lang.dat");

        using (new SecurityDisabler())
        {
          using (var definitionsWriter = File.OpenWrite(definitionsFilePath))
          {
            using (var sharedDataWriter = File.OpenWrite(sharedDataFilePath))
            {
              using (var languageDataWriter = File.OpenWrite(languageDataFilePath))
              {
                var stack = new Queue<Item>();
                var database = Factory.GetDatabase(databaseName);
                stack.Enqueue(database.GetRootItem());

                var definitions = new Dictionary<Guid, ItemInfo>();
                var sharedData = new ItemsSharedData();
                var languagesData = new ItemsLanguagesData();

                while (stack.Count > 0)
                {
                  var item = stack.Dequeue();

                  // enqueue children
                  foreach (var child in item.GetChildren().OfType<Item>().OrderBy(x => x.ID.Guid).ToArray())
                  {
                    stack.Enqueue(child);
                  }

                  // definition
                  AddDefinition(item, definitions);

                  // shared fields
                  AddShared(item, sharedData);

                  AddLanguages(item, languagesData);
                }

                ProtoBuf.Serializer.Serialize(definitionsWriter, definitions);
                ProtoBuf.Serializer.Serialize(sharedDataWriter, Wrap.Create(sharedData));
                ProtoBuf.Serializer.Serialize(languageDataWriter, Wrap.Create(languagesData));
              }
            }
          }
        }
      }

      Log.Info("Converting finished", this);
    }

    private static void AddDefinition(Item item, Dictionary<Guid, ItemInfo> definitions)
    {
      var definition = new ItemInfo
      {
        ID = item.ID.Guid,
        Name = item.Name,
        ParentID = item.ParentID.Guid,
        TemplateID = item.TemplateID.Guid
      };
      definitions.Add(item.ID.Guid, definition);
    }

    private static void AddShared(Item item, ItemsSharedData sharedData)
    {
      var sharedFields = new Dictionary<Guid, string>();
      sharedData.Add(item.ID.Guid, sharedFields);

      foreach (var field in item.Fields.OrderBy(x => x.ID.Guid).ToArray())
      {
        if (!field.InheritsValueFromOtherItem && field.Definition.IsShared)
        {
          sharedFields.Add(field.ID.Guid, field.Value);
        }
      }
    }

    private static void AddLanguages(Item item, ItemsLanguagesData languagesData)
    {
      var database = item.Database;

      var languages = new ItemLanguagesData();
      languagesData.Add(item.ID.Guid, languages);

      foreach (var language in item.Languages)
      {
        var languageName = language.Name;
        if (languageName != "en")
        {
          continue;
        }

        var languageFields = new FieldsData();
        languages.Add(languageName, languageFields);

        var version = database.GetItem(item.ID, language, Sitecore.Data.Version.Latest);
        foreach (var field in version.Fields.OrderBy(x => x.ID.Guid).ToArray())
        {
          if (!field.InheritsValueFromOtherItem && !field.Definition.IsShared)
          {
            languageFields.Add(field.ID.Guid, field.Value);
          }
        }
      }
    }
  }

  [ProtoContract]
  public class Wrap<T>
  {
    [ProtoMember(1)]
    public T Data;
  }

  public static class Wrap
  {
    public static Wrap<T> Create<T>(T data)
    {
      return new Wrap<T>
      {
        Data = data
      };
    }
  }
}