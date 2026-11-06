using ST10538419_CLDV7112_Practicium.Application.Cloud;
using ST10538419_CLDV7112_Practicium.Application.Farm;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Storage;

public static class DemoFarmSeed
{
    public static async Task EnsureAsync(IFarmTables tables, CancellationToken cancellationToken)
    {
        if (!tables.Configured)
        {
            return;
        }

        var readings = await tables.ListAsync(CloudResources.SensorReadings, 1, cancellationToken);
        if (readings.Count == 0)
        {
            await WriteReadingAsync(tables, "North", "22%", "26 C", cancellationToken);
            await WriteReadingAsync(tables, "North", "18%", "29 C", cancellationToken);
            await WriteReadingAsync(tables, "South", "31%", "24 C", cancellationToken);
        }

        var logs = await tables.ListAsync(CloudResources.EnvironmentalLogs, 1, cancellationToken);
        if (logs.Count == 0)
        {
            await WriteLogAsync(tables, "North", "Humidity", "41%", cancellationToken);
            await WriteLogAsync(tables, "North", "Wind", "12 km/h", cancellationToken);
            await WriteLogAsync(tables, "South", "Humidity", "55%", cancellationToken);
        }
    }

    private static Task WriteReadingAsync(
        IFarmTables tables,
        string field,
        string moisture,
        string temperature,
        CancellationToken cancellationToken)
    {
        var recordedAt = DateTime.UtcNow.ToString("O");
        return tables.UpsertAsync(
            CloudResources.SensorReadings,
            field,
            RowKey(),
            new Dictionary<string, string>
            {
                ["FieldName"] = field,
                ["Moisture"] = moisture,
                ["Temperature"] = temperature,
                ["RecordedAt"] = recordedAt
            },
            cancellationToken);
    }

    private static Task WriteLogAsync(
        IFarmTables tables,
        string field,
        string measure,
        string value,
        CancellationToken cancellationToken)
    {
        var recordedAt = DateTime.UtcNow.ToString("O");
        return tables.UpsertAsync(
            CloudResources.EnvironmentalLogs,
            field,
            RowKey(),
            new Dictionary<string, string>
            {
                ["FieldName"] = field,
                ["Measure"] = measure,
                ["Value"] = value,
                ["RecordedAt"] = recordedAt
            },
            cancellationToken);
    }

    private static string RowKey() => $"{DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks:D19}-{Guid.NewGuid():N}";
}
