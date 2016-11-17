namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Data.Templates;
  using Sitecore.Extensions.Object;

  public partial class CompositeDataProvider
  {
    /* Items.Templates part of DataProvider */

    [NotNull]
    private static readonly TemplateCollection EmptyTemplates = new TemplateCollection();

    [NotNull]
    private static readonly IEnumerable<ID> EmptyIds = new ID[0];

    public override IdCollection GetTemplateItemIds(CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var result = new IdCollection();
      var headIds = HeadProvider.GetTemplateItemIds(context) ?? new IdCollection();

      var readOnlyIds = ReadOnlyProviders
        .SelectMany(x => x
          .GetTemplateItemIds()?
          .Select(ID.Parse) ?? EmptyIds);

      var ids = headIds
        .Concat(readOnlyIds)        
        .GroupBy(x => x.Guid).Select(x => x.First()) // .Distinct()
        .ToArray();

      result.Add(ids);

      this.Trace(result, timer, context);
        
      return result;
    }

    public override TemplateCollection GetTemplates(CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var headTemplates = HeadProvider.GetTemplates(context) ?? EmptyTemplates;

      var readOnlyTemplates = ReadOnlyProviders
        .SelectMany(x => x
          .GetTemplates(context) ?? EmptyTemplates);

      var templates = headTemplates
        .Concat(readOnlyTemplates)
        .GroupBy(x => x.ID).Select(x => x.First()) // .Distinct()
        .ToArray();

      var result = new TemplateCollection();
      result.Reset(templates);

      this.Trace(result, timer, context);

      return result;
    }

    public override bool ChangeTemplate(ItemDefinition itemDefinition, TemplateChangeList changes, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif           
      this.Trace(true, timer, itemDefinition, context);

      if (HeadProvider.ChangeTemplate(itemDefinition, changes, context))
      {
        return true;
      }

      throw new NotImplementedException();
    }

    public override bool ChangeFieldSharing(TemplateField fieldDefinition, TemplateFieldSharing sharing, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif
      this.Trace(true, timer, context);

      if (HeadProvider.ChangeFieldSharing(fieldDefinition, sharing, context))
      {
        return true;
      }
                   
      throw new NotImplementedException();
    }
  }
}