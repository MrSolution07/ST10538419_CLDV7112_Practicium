using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.Shares;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ST10538419_CLDV7112_Practicium.Application.Cloud;
using ST10538419_CLDV7112_Practicium.Application.Configuration;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Storage;

public interface ICloudBootstrap
{
    Task StartAsync(CancellationToken cancellationToken);
}

public sealed class CloudBootstrap(IOptions<StorageOptions> options, ILogger<CloudBootstrap> logger) : ICloudBootstrap
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var storage = options.Value;
        if (!storage.BlobConfigured || !storage.QueueConfigured || !storage.TableConfigured || !storage.FileShareConfigured)
        {
            logger.LogInformation("Azure storage is not configured. Local run will not create cloud resources.");
            return;
        }

        var container = new BlobContainerClient(storage.BlobConnectionString, CloudResources.UploadContainer);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        var queue = new QueueClient(storage.QueueConnectionString, CloudResources.WorkQueue);
        await queue.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var table = new TableClient(storage.TableConnectionString, CloudResources.Table);
        await table.CreateIfNotExistsAsync(cancellationToken);

        var share = new ShareClient(storage.FileShareConnectionString, CloudResources.FileShare);
        await share.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        logger.LogInformation(
            "Azure storage is live. Container {Container}, queue {Queue}, table {Table}, share {Share}.",
            CloudResources.UploadContainer,
            CloudResources.WorkQueue,
            CloudResources.Table,
            CloudResources.FileShare);
    }
}
