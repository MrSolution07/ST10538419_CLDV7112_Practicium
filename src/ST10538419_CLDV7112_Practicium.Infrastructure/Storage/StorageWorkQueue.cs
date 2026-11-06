using Azure.Storage.Queues;
using ST10538419_CLDV7112_Practicium.Application.Configuration;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using Microsoft.Extensions.Options;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Storage;

public sealed class StorageWorkQueue(IOptions<StorageOptions> options) : IWorkQueue
{
    public bool Configured => options.Value.QueueConfigured;

    public async Task SendAsync(string queueName, string message, CancellationToken cancellationToken)
    {
        if (!Configured)
        {
            throw new InvalidOperationException("Queue storage is not configured. Set Storage:QueueConnectionString.");
        }

        var client = new QueueClient(options.Value.QueueConnectionString, queueName);
        await client.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        await client.SendMessageAsync(message, cancellationToken);
    }
}
