using ST10538419_CLDV7112_Practicium.Application.Cloud;

namespace ST10538419_CLDV7112_Practicium.Application.Farm;

public interface ITelemetryQuery
{
    Task<IReadOnlyList<SensorReading>> LatestAsync(string? fieldName, CancellationToken cancellationToken);
}

public sealed class TelemetryQuery(IFarmTables tables) : ITelemetryQuery
{
    public async Task<IReadOnlyList<SensorReading>> LatestAsync(string? fieldName, CancellationToken cancellationToken)
    {
        var rows = await tables.ListAsync(CloudResources.SensorReadings, 40, cancellationToken);
        IEnumerable<SensorReading> readings = rows.Select(Map);
        if (!string.IsNullOrWhiteSpace(fieldName))
        {
            var field = fieldName.Trim();
            readings = readings.Where(reading => reading.FieldName.Equals(field, StringComparison.OrdinalIgnoreCase));
        }

        return readings
            .OrderByDescending(reading => reading.RecordedAt)
            .Take(12)
            .ToList();
    }

    private static SensorReading Map(FarmRow row) => new(
        row.Values.GetValueOrDefault("FieldName") ?? row.PartitionKey,
        row.Values.GetValueOrDefault("Moisture") ?? "",
        row.Values.GetValueOrDefault("Temperature") ?? "",
        row.Values.GetValueOrDefault("RecordedAt") ?? "");
}
