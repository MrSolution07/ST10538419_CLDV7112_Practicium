using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.Shares;
using Azure.Storage.Queues;
using Azure.Data.Tables;
using ST10538419_CLDV7112_Practicium.Application.Cloud;
using ST10538419_CLDV7112_Practicium.Application.Configuration;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Storage;

public interface ICloudBootstrap
{
    Task StartAsync(CancellationToken cancellationToken);
}

public sealed class CloudBootstrap(
    IOptions<StorageOptions> options,
    IFarmTables tables,
    ILogger<CloudBootstrap> logger) : ICloudBootstrap
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var storage = options.Value;
        if (!storage.BlobConfigured || !storage.QueueConfigured || !storage.TableConfigured || !storage.FileShareConfigured)
        {
            logger.LogInformation("Azure storage is not configured. Local run will not create cloud resources.");
            return;
        }

        // Task (c): private container for drone images uploaded from the site.
        var container = new BlobContainerClient(storage.BlobConnectionString, CloudResources.DroneContainer);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        // Task (d): queue the irrigation function listens on.
        var queue = new QueueClient(storage.QueueConnectionString, CloudResources.IrrigationQueue);
        await queue.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        // Task (c) and (d): sensor readings, irrigation commands, environmental logs, and image audits.
        foreach (var tableName in new[]
        {
            CloudResources.SensorReadings,
            CloudResources.IrrigationCommands,
            CloudResources.EnvironmentalLogs,
            CloudResources.ImageAudits
        })
        {
            var table = new TableClient(storage.TableConnectionString, tableName);
            await table.CreateIfNotExistsAsync(cancellationToken);
        }

        var share = new ShareClient(storage.FileShareConnectionString, CloudResources.FileShare);
        await share.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        await DemoFarmSeed.EnsureAsync(tables, cancellationToken);

        logger.LogInformation(
            "Azure storage is live. Container {Container}, queue {Queue}.",
            CloudResources.DroneContainer,
            CloudResources.IrrigationQueue);
    }
}
