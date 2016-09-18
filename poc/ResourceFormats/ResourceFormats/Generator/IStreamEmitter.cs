using ResourceFormats.Model;

namespace ResourceFormats.Generator
{
  public interface IStreamEmitter
  {
    void Emit(IReadOnlyItem item, IEmitContext context);
  }
}