using Azure.Data.Tables;
using ST10538419_CLDV7112_Practicium.Application.Configuration;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using Microsoft.Extensions.Options;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Storage;

public sealed class FarmTableGateway(IOptions<StorageOptions> options) : IFarmTables
{
    public bool Configured => options.Value.TableConfigured;

    public async Task UpsertAsync(
        string tableName,
        string partitionKey,
        string rowKey,
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        if (!Configured)
        {
            throw new InvalidOperationException("Table storage is not configured. Set Storage:TableConnectionString.");
        }

        var client = new TableClient(options.Value.TableConnectionString, tableName);
        await client.CreateIfNotExistsAsync(cancellationToken);
        var entity = new TableEntity(partitionKey, rowKey);
        foreach (var (key, value) in values)
        {
            entity[key] = value;
        }

        await client.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);
    }

    public async Task<IReadOnlyList<FarmRow>> ListAsync(string tableName, int take, CancellationToken cancellationToken)
    {
        if (!Configured)
        {
            throw new InvalidOperationException("Table storage is not configured. Set Storage:TableConnectionString.");
        }

        var client = new TableClient(options.Value.TableConnectionString, tableName);
        await client.CreateIfNotExistsAsync(cancellationToken);
        var rows = new List<FarmRow>();
        await foreach (var entity in client.QueryAsync<TableEntity>(maxPerPage: take, cancellationToken: cancellationToken))
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in entity.Keys)
            {
                if (key is "PartitionKey" or "RowKey" or "Timestamp" or "odata.etag")
                {
                    continue;
                }

                values[key] = entity[key]?.ToString() ?? string.Empty;
            }

            rows.Add(new FarmRow(entity.PartitionKey, entity.RowKey, values));
            if (rows.Count >= take)
            {
                break;
            }
        }

        return rows;
    }
}
