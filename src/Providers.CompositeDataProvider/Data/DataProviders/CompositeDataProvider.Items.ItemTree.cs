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
      return HeadProvider.GetItemDefinition(itemId, context)
        ?? ReadOnlyProviders.FirstNotNull(x => x.GetItemDefinition(itemId));
    }

    public override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
    {
      return HeadProvider.GetParentID(itemDefinition, context)
        ?? ReadOnlyProviders.FirstNotNull(x => x.GetParentID(itemDefinition));
    }

    public override bool HasChildren(ItemDefinition itemDefinition, CallContext context)
    {
      return HeadProvider.HasChildren(itemDefinition, context) // speed optimization
        || DoGetChildIDs(itemDefinition, context).Any();
    }

    public override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
    {
      var list = new IDList();

      DoGetChildIDs(itemDefinition, context)
         .ForEach(x => list.Add(ID.Parse(x)));

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
        return pathId;
      }
        
      var ids = new Dictionary<ID, string>();
      foreach (var provider in ReadOnlyProviders)
      {
        var id = provider.ResolvePath(itemPath);
        if (ReferenceEquals(id, null) || ids.ContainsKey(id))
        {
          continue;
        }

        var path = provider.GetItemPath(id);
        ids.Add(id, path);

        // ResolvePath may return ID of the closest ancestor e.g. /sitecore/content's ID when /sitecore/content/home is missing
        // so here we check this situation 
        if (!path.Equals(itemPath, StringComparison.OrdinalIgnoreCase))
        {                   
          itemPath = itemPath.TrimEnd(" /".ToCharArray());
          path = path.TrimEnd(" /".ToCharArray());
          if (!path.Equals(itemPath, StringComparison.OrdinalIgnoreCase))
          {
            throw new NotImplementedException(itemPath + " != " + path);
          }
        }

        // The item may be moved or deleted in the HeadProvider so we check that before returning id
        var itemDefinition = HeadProvider.GetItemDefinition(id, context);
        if (itemDefinition == null)
        {
          // HeadProvider doesn't have anything for this item
          // so just return id
          return id;
        }

        // okay, we have something - check that the item wasn't deleted
        var parentId = HeadProvider.GetParentID(itemDefinition, context);
        if (parentId == ID.Undefined)
        {
          return null;
        }

        // okay, not deleted - then check if it was moved
        var readonlyParentId = provider.GetParentID(itemDefinition);
        if (readonlyParentId == parentId)
        {
          // not moved, it's okay
          return id;
        }

        // moved, return null
        return null;
      }

      return null;

      /*  
       * /sitecore/content/home path should resolve to all these cases
       * 
       * /sitecore{111}/content{222}/home{333}
       * /sitecore{111}/content{444}/home{333} (content item re-created)
       * /sitecore{111}/content{333}/home{222} (home moved to /sitecore and renamed to content, content is moved to new content and renamed to home)
       *  
       *  but only 1 is supported now
       *  
       */
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

      return list;
    }

    public override ID SelectSingleID(string query, CallContext context)
    {
      return ReadOnlyProviders.FirstNotNull(x => x
        .SelectIDs(query)?
        .Select(ID.Parse)
        .FirstOrDefault());
    }
  }
}