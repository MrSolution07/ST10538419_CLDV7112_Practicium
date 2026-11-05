using Azure.Storage.Queues;
using ST10538419_CLDV7112_Practicium.Application.Configuration;
using Microsoft.Extensions.Options;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Storage;

public sealed class AzureQueueStore(IOptions<StorageOptions> options) : IQueueStore
{
    public async Task SendAsync(string queueName, string message, CancellationToken cancellationToken)
    {
        var client = new QueueClient(options.Value.QueueConnectionString, queueName);
        await client.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        await client.SendMessageAsync(message, cancellationToken);
    }
}

public sealed class UnconfiguredQueueStore : IQueueStore
{
    public Task SendAsync(string queueName, string message, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("Queue storage is not configured. Set Storage:QueueConnectionString when the brief requires queues.");
}
