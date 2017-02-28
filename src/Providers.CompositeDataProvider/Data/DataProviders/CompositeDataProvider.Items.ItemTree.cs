namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Extensions.Enumerable;

  public partial class CompositeDataProvider
  {
    /* Items.ItemTree part of DataProvider */

    public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var definition = HeadProvider.GetItemDefinition(itemId, context)
        ?? ReadOnlyProviders.FirstNotNull(x => x.GetItemDefinition(itemId));

#if DEBUG
      this.Trace(definition, timer, itemId, context);
#endif

      return definition;
    }

    public override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var parentId = HeadProvider.GetParentID(itemDefinition, context)
        ?? ReadOnlyProviders.FirstNotNull(x => x.GetParentID(itemDefinition));

#if DEBUG
      this.Trace(parentId, timer, itemDefinition, context);
#endif

      return parentId;
    }

    public override bool HasChildren(ItemDefinition itemDefinition, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var hasChildren = HeadProvider.HasChildren(itemDefinition, context) // speed optimization
        || DoGetChildIDs(itemDefinition, context).Any();

#if DEBUG
      this.Trace(hasChildren, timer, itemDefinition, context);
#endif

      return hasChildren;
    }

    [NotNull]
    public override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var list = new IDList();

      DoGetChildIDs(itemDefinition, context)
         .ForEach(x => list.Add(ID.Parse(x)));

#if DEBUG
      this.Trace(list, timer, itemDefinition, context);
#endif

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
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif
      ID pathId;
      if (ID.TryParse(itemPath, out pathId))
      {
#if DEBUG
        this.Trace(pathId, timer, itemPath, context);
#endif

        return pathId;
      }

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

#if DEBUG
      this.Trace(pathId, timer, itemPath, context);
#endif

      return pathId;
    }

    private ID ResolvePath(ID parentId, string[] pathSegments, int segmentIndex, CallContext context)
    {
      if (segmentIndex >= pathSegments.Length)
      {
        return parentId;
      }
                        
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

          var pathId = ResolvePath(childId, pathSegments, segmentIndex + 1, context);
          if (pathId == (ID)null)
          {
            continue;
          }

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
        var pathId = ResolveHeadPath(childId, pathSegments, segmentIndex + 1, context);
        if (pathId != (ID)null)
        {
          return pathId;
        }
      }

      return null;
    }

    public override IDList SelectIDs(string query, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var list = new IDList();

      ReadOnlyProviders
        .SelectMany(x => x
          .SelectIDs(query)?
          .Select(ID.Parse) ?? EmptyIds)

        .GroupBy(x => x).Select(x => x.First()) // .Distinct()
        .ForEach(x => list.Add(x));

#if DEBUG
      this.Trace(list, timer, query, context);
#endif

      return list;
    }

    public override ID SelectSingleID(string query, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var id = ReadOnlyProviders.FirstNotNull(x => x
        .SelectIDs(query)?
        .Select(ID.Parse)
        .FirstOrDefault());

#if DEBUG
      this.Trace(id, timer, query, context);
#endif

      return id;
    }
  }
}