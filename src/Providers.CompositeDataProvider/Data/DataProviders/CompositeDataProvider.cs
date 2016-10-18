namespace Sitecore.Data.DataProviders
{
  public sealed partial class CompositeDataProvider : AbstractCompositeDataProvider
  {
    [UsedImplicitly]
    public CompositeDataProvider([NotNull] string databaseName) : base(databaseName)
    {
    }
                       
    public CompositeDataProvider([NotNull] string databaseName, [NotNull] params DataProvider[] providers) : base(databaseName, providers)
    {                                                                          
    }
  }
}