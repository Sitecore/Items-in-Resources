namespace IR.Convert
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using System.Linq;
  using IR.Data;
  using IR.Data.DataFormat;
  using ProtoBuf;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Database.Items;
  using Sitecore.Diagnostics.SqlDataProvider.Items;

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
      Assert.ArgumentCondition(ExceptionHelper.Try(() => new SqlConnectionStringBuilder(ConnectionString)), nameof(ConnectionString), "$The specified connection string");
                  
      using (var definitionsWriter = File.OpenWrite(Path.Combine(OutputDirectory, $"{DatabaseName}.definitions.dat")))
      {
        using (var sharedDataWriter = File.OpenWrite(Path.Combine(OutputDirectory, $"{DatabaseName}.data.shared.dat")))
        {
          using (var languageDataWriter = File.OpenWrite(Path.Combine(OutputDirectory, $"{DatabaseName}.data.lang.dat")))
          {
            var definitions = new ItemsDefinitions();
            var itemsSharedData = new ItemsSharedData();
            var itemsLanguagesData = new ItemsLanguagesData();

            var context = ItemManager.Initialize(ConnectionString);
            var queue = new Queue<Item>();
            queue.Enqueue(context.GetItems().First(x => x.ID == ItemIDs.RootItemID));
            while(queue.Count > 0)
            {
              var item = queue.Dequeue();

              // enqueue children        
              var parentId = item.ID;
              var children = context.GetItems()
                .Where(x => x.ParentID == parentId)
                .ToArray()
                .OrderBy(x => x.ID)
                .ToArray();

              foreach (var child in children)
              {
                queue.Enqueue(child);
              }

              // definition
              AddDefinition(item, definitions);

              // shared fields
              AddShared(item, itemsSharedData);

              // unversioned and versioned-1 fields
              AddLanguages(item, itemsLanguagesData);
            }

            Serializer.Serialize(definitionsWriter, definitions);
            Serializer.Serialize(sharedDataWriter, ProtobufWrap.Create(itemsSharedData));
            Serializer.Serialize(languageDataWriter, ProtobufWrap.Create(itemsLanguagesData));
          }
        }
      }
    }

    private static void AddDefinition(Item item, Dictionary<Guid, ItemInfo> definitions)
    {
      var definition = new ItemInfo
      {
        ID = item.ID,
        Name = item.Name,
        ParentID = item.ParentID,
        TemplateID = item.TemplateID
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
        FieldsData languageFields;
        if (!languages.TryGetValue(languageName, out languageFields))
        {
          languageFields = new FieldsData();
          languages.Add(languageName, languageFields);
        }

        foreach (var field in pair.Value.OrderBy(x => Guid.Parse(x.Key)).ToArray())
        {
          languageFields.Add(Guid.Parse(field.Key), field.Value);
        }
      }

      foreach (var pair in item.Fields.Versioned)
      {
        var languageName = pair.Key;    
        FieldsData languageFields;
        if (!languages.TryGetValue(languageName, out languageFields))
        {
          languageFields = new FieldsData();
          languages.Add(languageName, languageFields);
        }

        if (!pair.Value.Any())
        {
          continue;
        }

        var lastVersion = pair.Value.Keys.Max(z => z);
        var versionedFields = pair.Value.Single(x => x.Key == lastVersion).Value;
        if (versionedFields == null || !versionedFields.Any())
        {
          continue;
        }

        foreach (var field in versionedFields.OrderBy(x => Guid.Parse(x.Key)).ToArray())
        {
          languageFields.Add(Guid.Parse(field.Key), field.Value);
        }
      }
    }
  }
}