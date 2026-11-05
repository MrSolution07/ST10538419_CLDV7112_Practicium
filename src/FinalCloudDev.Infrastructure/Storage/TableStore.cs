using Azure.Data.Tables;
using FinalCloudDev.Application.Configuration;
using Microsoft.Extensions.Options;

namespace FinalCloudDev.Infrastructure.Storage;

public sealed class AzureTableStore(IOptions<StorageOptions> options) : ITableStore
{
    public async Task UpsertAsync(
        string tableName,
        string partitionKey,
        string rowKey,
        IReadOnlyDictionary<string, object> values,
        CancellationToken cancellationToken)
    {
        var client = new TableClient(options.Value.TableConnectionString, tableName);
        await client.CreateIfNotExistsAsync(cancellationToken);
        var entity = new TableEntity(partitionKey, rowKey);
        foreach (var (key, value) in values)
        {
            entity[key] = value;
        }

        await client.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);
    }
}

public sealed class UnconfiguredTableStore : ITableStore
{
    public Task UpsertAsync(string tableName, string partitionKey, string rowKey, IReadOnlyDictionary<string, object> values, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("Table storage is not configured. Set Storage:TableConnectionString when the brief requires tables.");
}
