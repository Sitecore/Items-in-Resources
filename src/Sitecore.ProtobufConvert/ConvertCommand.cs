namespace ProtobufConvert
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using System.Linq;
  using ProtoBuf;
  using Sitecore.Data;
  using Sitecore.Data.ProtobufDataProvider;
  using Sitecore.Data.ProtobufDataProvider.DataFormat;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Database.Items;
  using Sitecore.Diagnostics.SqlDataProvider.Items;
  using Sitecore.Extensions.Dictionary;

  public class ConvertCommand
  {
    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public string OutputDirectory { get; set; }

    public void Execute()
    {
      Assert.ArgumentNotNullOrEmpty(ConnectionString, nameof(ConnectionString));
      Assert.ArgumentNotNullOrEmpty(DatabaseName, nameof(DatabaseName));
      Assert.ArgumentNotNullOrEmpty(OutputDirectory, nameof(OutputDirectory));
      Assert.ArgumentCondition(Directory.Exists(OutputDirectory), nameof(OutputDirectory), $"The specified directory does not exist: {OutputDirectory}");
      Assert.ArgumentCondition(Try(() => new SqlConnectionStringBuilder(ConnectionString)), nameof(ConnectionString), "$The specified connection string");

      using (var definitionsWriter = File.OpenWrite(Path.Combine(OutputDirectory, $"{DatabaseName}.items.dat")))
      {
        var definitions = new ItemDefinitions();
        var itemsSharedData = new ItemsSharedData();
        var itemsLanguagesData = new ItemsLanguagesData();

        var context = ItemManager.Initialize(ConnectionString);

        var counter = 0;
        Console.WriteLine("Processing items...");
        foreach (var item in GetItems(context))
        {
          // definition
          AddDefinition(item, definitions);

          // shared fields
          AddShared(item, itemsSharedData);

          // unversioned and versioned-1 fields
          AddLanguages(item, itemsLanguagesData);

          Console.WriteLine($"{++counter:D5}. {GetItemPath(definitions, item.ID)}");
        }

        Console.WriteLine("Serializing...");
        Serializer.Serialize(definitionsWriter, new ItemsData
        {
          Definitions = definitions,
          SharedData = itemsSharedData,
          LanguageData = itemsLanguagesData
        });
      }
    }

    private string GetItemPath(ItemDefinitions definitions, Guid itemId)
    {
      if (itemId == Guid.Empty)
      {
        return "";
      }

      var definition = definitions[itemId];
      var itemName = definition.Name;
      var parenItemPath = GetItemPath(definitions, definition.ParentID);

      return $"{parenItemPath}/{itemName}";
    }

    private IEnumerable<Item> GetItems(ItemContext context)
    {
      var root = context.GetItems().FirstOrDefault(i => i.ID == ItemIDs.RootItemID);
      if (root != null)
      {
        return EnumerateTree(context, root);
      }

      return context.GetItems();
    }

    private IEnumerable<Item> EnumerateTree(ItemContext context, Item root)
    {
      var queue = new Queue<Item>();
      queue.Enqueue(root);
      while (queue.Count > 0)
      {
        var item = queue.Dequeue();
        var children = context.GetItems()
          .Where(x => x.ParentID == item.ID)
          .ToArray()
          .OrderBy(x => x.ID)
          .ToArray();

        foreach (var child in children)
        {
          queue.Enqueue(child);
        }

        yield return item;
      }
    }

    private static void AddDefinition(Item item, ItemDefinitions definitions)
    {
      var definition = new ItemRecord
      {
        ID = item.ID,
        Name = item.Name,
        ParentID = item.ParentID,
        TemplateID = item.TemplateID,
      };

      definitions.Add(item.ID, definition);
    }

    private static void AddShared(Item item, ItemsSharedData itemsSharedData)
    {
      var sharedFields = new FieldsData();
      itemsSharedData.Add(item.ID, sharedFields);

      foreach (var field in item.Fields.Shared.OrderBy(x => Guid.Parse(x.Key)).ToArray())
      {
        sharedFields.Add(Guid.Parse(field.Key), field.Value);
      }
    }

    private static void AddLanguages(Item item, ItemsLanguagesData languagesData)
    {
      var languages = new ItemLanguagesData();
      languagesData.Add(item.ID, languages);

      foreach (var pair in item.Fields.Unversioned)
      {
        var languageName = pair.Key;
        VersionsData languageFields;
        if (!languages.TryGetValue(languageName, out languageFields))
        {
          languageFields = new VersionsData();
          languages.Add(languageName, languageFields);
        }

        var unversionedFields = languageFields.TryGetValue(0);
        if (unversionedFields == null)
        {
          unversionedFields = new FieldsData();
          languageFields.Add(0, unversionedFields);
        }

        foreach (var field in pair.Value.OrderBy(x => Guid.Parse(x.Key)).ToArray())
        {
          unversionedFields.Add(Guid.Parse(field.Key), field.Value);
        }
      }

      foreach (var pair in item.Fields.Versioned)
      {
        var languageName = pair.Key;
        VersionsData languageFields;
        if (!languages.TryGetValue(languageName, out languageFields))
        {
          languageFields = new VersionsData();
          languages.Add(languageName, languageFields);
        }

        foreach (var versionPair in pair.Value)                         
        {
          var versionNumber = versionPair.Key;
          var version = languageFields.TryGetValue(versionNumber);
          if (version == null)
          {
            version = new FieldsData();
            languageFields.Add(versionNumber, version);
          }

          var versionFields = versionPair.Value
            .Select(x => CreatePair(Guid.Parse(x.Key), x.Value))
            .OrderBy(x => x.Key);

          foreach (var field in versionFields)
          {
            version.Add(field.Key, field.Value);
          }
        }               
      }
    }

    private static KeyValuePair<TK, TV> CreatePair<TK, TV>(TK parse, TV argValue)
    {
      return new KeyValuePair<TK, TV>(parse, argValue);
    }

    private static bool Try([NotNull] Action action)
    {
      Assert.ArgumentNotNull(action, nameof(action));

      try
      {
        action();

        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
