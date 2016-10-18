namespace Sitecore.Data.DataProviders
{
  using System;
  using System.IO;
  using System.Linq;
  using Sitecore.Extensions.Enumerable;

  public partial class CompositeDataProvider
  {
    /* Media part of DataProvider */

    public override bool BlobStreamExists(Guid blobId, CallContext context)
    {
      return HeadProvider.BlobStreamExists(blobId, context) ||
        ReadOnlyProviders.Any(x => x.BlobStreamExists(blobId, context));
    }

    public override Stream GetBlobStream(Guid blobId, CallContext context)
    {
      return HeadProvider.GetBlobStream(blobId, context) ??
        ReadOnlyProviders.FirstNotNull(x => x.GetBlobStream(blobId, context));
    }

    public override bool SetBlobStream(Stream stream, Guid blobId, CallContext context)
    {
      return HeadProvider.SetBlobStream(stream, blobId, context) ||
        ReadOnlyProviders.Any(x => x.SetBlobStream(stream, blobId, context));
    }

    public override bool RemoveBlobStream(Guid blobId, CallContext context)
    {
      return HeadProvider.RemoveBlobStream(blobId, context);
    }
  }
}