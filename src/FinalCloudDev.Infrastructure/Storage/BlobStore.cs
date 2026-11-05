using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FinalCloudDev.Application.Configuration;
using Microsoft.Extensions.Options;

namespace FinalCloudDev.Infrastructure.Storage;

public sealed class AzureBlobStore(IOptions<StorageOptions> options) : IBlobStore
{
    public async Task<string> UploadAsync(
        string container,
        string blobName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken)
    {
        var client = new BlobContainerClient(options.Value.BlobConnectionString, container);
        await client.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        var blob = client.GetBlobClient(blobName);
        await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
        return blob.Uri.ToString();
    }
}

public sealed class UnconfiguredBlobStore : IBlobStore
{
    public Task<string> UploadAsync(string container, string blobName, Stream content, string contentType, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("Blob storage is not configured. Set Storage:BlobConnectionString when the brief requires blobs.");
}
