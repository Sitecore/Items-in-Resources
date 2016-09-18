namespace ResourceFormats.Generator
{
  public interface IEmitContext
  {
    bool HasFacet<T>();

    void AddFacet<T>(T facet);

    bool TryGetFacet<T>(out T facet);
  }
}