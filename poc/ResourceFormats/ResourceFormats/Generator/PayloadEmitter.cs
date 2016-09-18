using System;
using System.IO;
using System.Text;

using ResourceFormats.Model;

namespace ResourceFormats.Generator
{
  public class PayloadEmitter : IStreamEmitter, IDisposable

  {
    private readonly Stream targetStream;

    private readonly long positionBase;

    private readonly BinaryWriter writer;

    public PayloadEmitter(Stream targetStream)
    {
      this.targetStream = targetStream;
      this.positionBase = this.targetStream.Position;
      this.writer = new BinaryWriter(this.targetStream, Encoding.UTF8, true);
    }

    private long Position => this.targetStream.Position - this.positionBase;

    public void Emit(IReadOnlyItem item, IEmitContext context)
    {
      context.AddFacet(new PayloadOffset(this.Position));
      this.WriteItem(item);
    }

    private void WriteItem(IReadOnlyItem item)
    {
      this.writer.Write("Hello");
    }

    public void Dispose()
    {
      this.targetStream.Dispose();
      this.writer.Dispose();
    }
  }
}