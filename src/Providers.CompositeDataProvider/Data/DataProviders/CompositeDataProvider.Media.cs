namespace Sitecore.Data.DataProviders
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using Sitecore.Extensions.Enumerable;
  using Sitecore.Extensions.Object;

  public partial class CompositeDataProvider
  {
    /* Media part of DataProvider */

    public override bool BlobStreamExists(Guid blobId, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var exists = HeadProvider?.BlobStreamExists(blobId, context) ?? false;

#if DEBUG
      this.Trace(exists, timer, blobId, context);
#endif

      return exists;
    }

    public override Stream GetBlobStream(Guid blobId, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var stream = HeadProvider?.GetBlobStream(blobId, context);

#if DEBUG
      this.Trace(stream, timer, blobId, context);
#endif

      return stream;
    }

    public override bool SetBlobStream(Stream stream, Guid blobId, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var set = HeadProvider?.SetBlobStream(stream, blobId, context) ?? false;

#if DEBUG
      this.Trace(set, timer, stream, blobId, context);
#endif

      return set;
    }

    public override bool RemoveBlobStream(Guid blobId, CallContext context)
    {
#if DEBUG
      var timer = Stopwatch.StartNew();
#endif

      var removed = HeadProvider?.RemoveBlobStream(blobId, context) ?? false;

#if DEBUG
      this.Trace(removed, timer, blobId, context);
#endif

      return removed;
    }
  }
}