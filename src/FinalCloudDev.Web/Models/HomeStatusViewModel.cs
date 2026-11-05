namespace FinalCloudDev.Web.Models;

public sealed class HomeStatusViewModel
{
    public required string SiteName { get; init; }

    public required string Tagline { get; init; }

    public required string DatabaseProvider { get; init; }

    public required int ActiveRecords { get; init; }

    public required bool BlobConfigured { get; init; }

    public required bool QueueConfigured { get; init; }

    public required bool TableConfigured { get; init; }

    public required bool FileShareConfigured { get; init; }
}
