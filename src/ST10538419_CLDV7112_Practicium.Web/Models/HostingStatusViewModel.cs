namespace ST10538419_CLDV7112_Practicium.Web.Models;

public sealed class HostingStatusViewModel
{
    public required string Host { get; init; }

    public required string DatabaseProvider { get; init; }

    public required bool BlobReady { get; init; }

    public required bool QueueReady { get; init; }

    public required bool TableReady { get; init; }

    public required bool LogsReady { get; init; }
}
