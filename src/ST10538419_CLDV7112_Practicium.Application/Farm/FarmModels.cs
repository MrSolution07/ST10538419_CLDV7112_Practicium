namespace ST10538419_CLDV7112_Practicium.Application.Farm;

public static class CommandTypes
{
    public const string Irrigate = "irrigate";

    public const string SensorAlert = "sensor-alert";

    public static bool IsKnown(string? commandType) =>
        commandType is Irrigate or SensorAlert;
}

public sealed record FarmRow(string PartitionKey, string RowKey, IReadOnlyDictionary<string, string> Values);

public sealed record SensorReading(string FieldName, string Moisture, string Temperature, string RecordedAt);

public sealed record EnvironmentalLog(string FieldName, string Measure, string Value, string RecordedAt);

public sealed record IrrigationMessage(string CommandId, Guid DeviceId, string CommandType);

public sealed record DroneImageItem(string Name, string ContentType);

public sealed record DroneImageContent(Stream Content, string ContentType);

public sealed record GalleryImage(string Name, string AuditStatus, string RecordedAt);

public sealed record FarmGalleryModel(bool Configured, string? Message, IReadOnlyList<GalleryImage> Images);

public sealed record LogList(bool Configured, string? Message, IReadOnlyList<EnvironmentalLog> Logs);

public sealed record FieldCall<T>(bool Configured, bool Succeeded, T? Value, string? Message);
