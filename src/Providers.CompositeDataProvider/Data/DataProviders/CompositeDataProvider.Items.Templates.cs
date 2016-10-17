namespace Sitecore.Data.DataProviders
{
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.Templates;
  using Sitecore.Extensions;
  using Sitecore.Extensions.Enumerable;

  public partial class CompositeDataProvider
  {
    /* Items.Templates part of DataProvider */

    public override IdCollection GetTemplateItemIds(CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetTemplateItemIds(context));
    }

    public override TemplateCollection GetTemplates(CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetTemplates(context));
    }

    public override bool ChangeTemplate(ItemDefinition itemDefinition, TemplateChangeList changes, CallContext context)
    {
      return Providers.Any(x => x.ChangeTemplate(itemDefinition, changes, context));
    }

    public override bool ChangeFieldSharing(TemplateField fieldDefinition, TemplateFieldSharing sharing, CallContext context)
    {
      return Providers.Any(x => x.ChangeFieldSharing(fieldDefinition, sharing, context));
    }           
  }
}