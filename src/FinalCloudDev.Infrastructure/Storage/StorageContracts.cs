namespace FinalCloudDev.Infrastructure.Storage;

public interface IBlobStore
{
    Task<string> UploadAsync(
        string container,
        string blobName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken);
}

public interface IQueueStore
{
    Task SendAsync(string queueName, string message, CancellationToken cancellationToken);
}

public interface ITableStore
{
    Task UpsertAsync(string tableName, string partitionKey, string rowKey, IReadOnlyDictionary<string, object> values, CancellationToken cancellationToken);
}

public interface IFileShareStore
{
    Task UploadAsync(string shareName, string fileName, Stream content, CancellationToken cancellationToken);
}
