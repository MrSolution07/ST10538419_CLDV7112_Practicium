namespace FinalCloudDev.Application.Configuration;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string? BlobConnectionString { get; set; }

    public string? QueueConnectionString { get; set; }

    public string? TableConnectionString { get; set; }

    public string? FileShareConnectionString { get; set; }

    public bool BlobConfigured => !string.IsNullOrWhiteSpace(BlobConnectionString);

    public bool QueueConfigured => !string.IsNullOrWhiteSpace(QueueConnectionString);

    public bool TableConfigured => !string.IsNullOrWhiteSpace(TableConnectionString);

    public bool FileShareConfigured => !string.IsNullOrWhiteSpace(FileShareConnectionString);
}
