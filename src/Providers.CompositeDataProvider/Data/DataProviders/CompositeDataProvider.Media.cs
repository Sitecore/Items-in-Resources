namespace Sitecore.Data.DataProviders
{
  using System;
  using System.IO;
  using System.Linq;
  using Sitecore.Extensions;

  public partial class CompositeDataProvider
  {
    /* Media part of DataProvider */

    public override bool BlobStreamExists(Guid blobId, CallContext context)
    {
      return Providers.Any(x => x.BlobStreamExists(blobId, context));
    }

    public override Stream GetBlobStream(Guid blobId, CallContext context)
    {
      return Providers.FirstNotNull(x => x.GetBlobStream(blobId, context));
    }

    public override bool SetBlobStream(Stream stream, Guid blobId, CallContext context)
    {
      return Providers.Any(x => x.SetBlobStream(stream, blobId, context));
    }

    public override bool RemoveBlobStream(Guid blobId, CallContext context)
    {
      return Providers.Any(x => x.RemoveBlobStream(blobId, context));
    }
  }
}