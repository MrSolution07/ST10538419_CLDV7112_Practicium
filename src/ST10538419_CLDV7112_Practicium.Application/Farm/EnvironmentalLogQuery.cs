using ST10538419_CLDV7112_Practicium.Application.Cloud;

namespace ST10538419_CLDV7112_Practicium.Application.Farm;

public interface IEnvironmentalLogQuery
{
    Task<LogList> LatestAsync(CancellationToken cancellationToken);
}

public sealed class EnvironmentalLogQuery(IFarmTables tables) : IEnvironmentalLogQuery
{
    public async Task<LogList> LatestAsync(CancellationToken cancellationToken)
    {
        if (!tables.Configured)
        {
            return new LogList(false, "Environmental logs are unavailable until table storage is configured.", []);
        }

        var rows = await tables.ListAsync(CloudResources.EnvironmentalLogs, 40, cancellationToken);
        var logs = rows
            .Select(row => new EnvironmentalLog(
                row.Values.GetValueOrDefault("FieldName") ?? row.PartitionKey,
                row.Values.GetValueOrDefault("Measure") ?? "",
                row.Values.GetValueOrDefault("Value") ?? "",
                row.Values.GetValueOrDefault("RecordedAt") ?? ""))
            .OrderByDescending(log => log.RecordedAt)
            .Take(12)
            .ToList();

        return new LogList(true, null, logs);
    }
}
