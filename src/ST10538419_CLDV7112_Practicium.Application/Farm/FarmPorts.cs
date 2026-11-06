namespace ST10538419_CLDV7112_Practicium.Application.Farm;

public interface IFarmTables
{
    bool Configured { get; }

    Task UpsertAsync(
        string tableName,
        string partitionKey,
        string rowKey,
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<FarmRow>> ListAsync(string tableName, int take, CancellationToken cancellationToken);
}

public interface IWorkQueue
{
    bool Configured { get; }

    Task SendAsync(string queueName, string message, CancellationToken cancellationToken);
}

public interface IDroneImageCabinet
{
    bool Configured { get; }

    Task UploadAsync(string blobName, Stream content, string contentType, CancellationToken cancellationToken);

    Task<IReadOnlyList<DroneImageItem>> ListAsync(CancellationToken cancellationToken);

    Task<DroneImageContent?> OpenAsync(string blobName, CancellationToken cancellationToken);
}

public interface IFieldFunctions
{
    bool Configured { get; }

    Task<FieldCall<IReadOnlyList<SensorReading>>> GetLatestAsync(string? fieldName, CancellationToken cancellationToken);

    Task<FieldCall<string>> SendCommandAsync(Guid deviceId, string commandType, CancellationToken cancellationToken);
}
