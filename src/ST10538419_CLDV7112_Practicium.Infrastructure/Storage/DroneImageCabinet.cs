using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ST10538419_CLDV7112_Practicium.Application.Cloud;
using ST10538419_CLDV7112_Practicium.Application.Configuration;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using Microsoft.Extensions.Options;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Storage;

public sealed class DroneImageCabinet(IOptions<StorageOptions> options) : IDroneImageCabinet
{
    public bool Configured => options.Value.BlobConfigured;

    public async Task UploadAsync(string blobName, Stream content, string contentType, CancellationToken cancellationToken)
    {
        var blob = await BlobAsync(blobName, cancellationToken);
        await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<DroneImageItem>> ListAsync(CancellationToken cancellationToken)
    {
        var container = Container();
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        var items = new List<DroneImageItem>();
        await foreach (var blob in container.GetBlobsAsync(cancellationToken: cancellationToken))
        {
            items.Add(new DroneImageItem(blob.Name, blob.Properties.ContentType ?? "application/octet-stream"));
        }

        return items;
    }

    public async Task<DroneImageContent?> OpenAsync(string blobName, CancellationToken cancellationToken)
    {
        var blob = await BlobAsync(blobName, cancellationToken);
        if (!await blob.ExistsAsync(cancellationToken))
        {
            return null;
        }

        var download = await blob.DownloadStreamingAsync(cancellationToken: cancellationToken);
        var type = download.Value.Details.ContentType ?? "application/octet-stream";
        return new DroneImageContent(download.Value.Content, type);
    }

    private async Task<BlobClient> BlobAsync(string blobName, CancellationToken cancellationToken)
    {
        if (!Configured)
        {
            throw new InvalidOperationException("Blob storage is not configured. Set Storage:BlobConnectionString.");
        }

        var container = Container();
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        return container.GetBlobClient(blobName);
    }

    private BlobContainerClient Container() =>
        new(options.Value.BlobConnectionString, CloudResources.DroneContainer);
}
