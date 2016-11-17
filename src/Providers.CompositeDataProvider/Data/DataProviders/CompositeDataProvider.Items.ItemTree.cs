namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Extensions.Object;

  public partial class CompositeDataProvider
  {
    /* Items.ItemTree part of DataProvider */

    public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
    {
      var definition = HeadProvider.GetItemDefinition(itemId, context)
        ?? ReadOnlyProviders.FirstNotNull(x => x.GetItemDefinition(itemId));

      this.Trace(definition, null, itemId, context.DataManager.Database.Name);

      return definition;
    }

    public override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
    {
      var parentId = HeadProvider.GetParentID(itemDefinition, context)
        ?? ReadOnlyProviders.FirstNotNull(x => x.GetParentID(itemDefinition));

      this.Trace(parentId, null, itemDefinition.ID, context.DataManager.Database.Name);

      return parentId;
    }

    public override bool HasChildren(ItemDefinition itemDefinition, CallContext context)
    {
      var hasChildren = HeadProvider.HasChildren(itemDefinition, context) // speed optimization
        || DoGetChildIDs(itemDefinition, context).Any();

      this.Trace(hasChildren, null, itemDefinition.ID, context.DataManager.Database.Name);

      return hasChildren;
    }

    public override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
    {
      var list = new IDList();

      DoGetChildIDs(itemDefinition, context)
         .ForEach(x => list.Add(ID.Parse(x)));

      this.Trace(list, null, itemDefinition.ID, context.DataManager.Database.Name);

      return list;
    }

    [NotNull]
    private IEnumerable<ID> DoGetChildIDs(ItemDefinition itemDefinition, CallContext context)
    {
      var headChildIDs = HeadProvider
        .GetChildIDs(itemDefinition, context)?
        .Cast<ID>().ToArray() ?? EmptyIds;

      var readOnlyChildIDs = ReadOnlyProviders
        .SelectMany(x => x
          .GetChildIDs(itemDefinition)?
          .Select(ID.Parse) ?? EmptyIds);

      var childIDs = headChildIDs.Concat(readOnlyChildIDs) // .Join()
        .GroupBy(x => x.Guid).Select(x => x.First()); // .Distinct()

      // deleted or moved out items must be get off the list
      var itemId = itemDefinition.ID;
      foreach (var childID in childIDs)
      {
        var parentId = HeadProvider.GetParentID(new ItemDefinition(childID, string.Empty, ID.Null, ID.Null), context);
        if (ReferenceEquals(parentId, null) || parentId == itemId)
        {
          yield return childID;
        }
      }
    }

    public override ID ResolvePath(string itemPath, CallContext context)
    {
      ID pathId;
      if (ID.TryParse(itemPath, out pathId))
      {
        this.Trace(pathId, null, itemPath, context.DataManager.Database.Name);

        return pathId;
      }

      var timer = Stopwatch.StartNew();
      if (!itemPath.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
      {
        return null;
      }
                                             
      itemPath = itemPath.TrimEnd("/".ToCharArray());
      if (itemPath.Length == "/sitecore".Length)
      {
        return ID.Parse(ItemIDs.RootItemID);
      }

      if (itemPath["/sitecore".Length] != '/')
      {
        throw new ArgumentException($"itemPath must start with /sitecore/ (actual: {itemPath})");
      }

      var rootId = Sitecore.ItemIDs.RootID;
      var pathSegments = itemPath.Substring("/sitecore/".Length).Split('/');
      pathId = ResolvePath(rootId, pathSegments, 0, context);
      
      this.Trace(pathId, timer, itemPath, context.DataManager.Database.Name);

      return pathId;
    }        

    private ID ResolvePath(ID parentId, string[] pathSegments, int segmentIndex, CallContext context)
    {
      if (segmentIndex >= pathSegments.Length)
      {
        return parentId;
      }

      var timer = Stopwatch.StartNew();
      var segmentName = pathSegments[segmentIndex];
      foreach (var provider in ReadOnlyProviders)
      {
        var children = provider.GetChildIdsByName(segmentName, parentId);
        if (children == null)
        {
          continue;
        }

        foreach (var childId in children)
        {
          // TODO: refactor that in kernel
          var headParentId = HeadProvider.GetParentID(new ItemDefinition(childId, "--fake--", ID.Null, ID.Null), context);
          if (headParentId != (ID)null && headParentId != parentId)
          {
            continue;
          }

          var pathId = this.ResolvePath(childId, pathSegments, segmentIndex + 1, context);
          if (pathId == (ID)null)
          {
            continue;
          }

          this.Trace(pathId, timer, segmentName, context.DataManager.Database.Name);

          return pathId;
        }
      }

      return ResolveHeadPath(parentId, pathSegments, segmentIndex, context);
    }

    private ID ResolveHeadPath(ID parentId, string[] pathSegments, int segmentIndex, CallContext context)
    {
      if (segmentIndex >= pathSegments.Length)
      {
        return parentId;
      }

      var segmentName = pathSegments[segmentIndex];
      var children = HeadProviderEx.GetChildIdsByName(segmentName, parentId);
      foreach (var childId in children)
      {
        var pathId = ResolvePath(childId, pathSegments, segmentIndex + 1, context);
        if (pathId != (ID)null)
        {
          return pathId;
        }
      }

      return null;
    }

    public override IDList SelectIDs(string query, CallContext context)
    {
      var list = new IDList();

      ReadOnlyProviders
        .SelectMany(x => x
          .SelectIDs(query)?
          .Select(ID.Parse) ?? EmptyIds)

        .GroupBy(x => x).Select(x => x.First()) // .Distinct()
        .ForEach(x => list.Add(x));

      this.Trace(list, null, query, context.DataManager.Database.Name);

      return list;
    }

    public override ID SelectSingleID(string query, CallContext context)
    {
      var id = ReadOnlyProviders.FirstNotNull(x => x
        .SelectIDs(query)?
        .Select(ID.Parse)
        .FirstOrDefault());

      this.Trace(id, null, query, context.DataManager.Database.Name);

      return id;
    }
  }
}