using ST10538419_CLDV7112_Practicium.Application.Cloud;

namespace ST10538419_CLDV7112_Practicium.Application.Farm;

public interface IImageAuditProcessor
{
    Task RecordAsync(string blobName, long size, CancellationToken cancellationToken);
}

public sealed class ImageAuditProcessor(IFarmTables tables) : IImageAuditProcessor
{
    public async Task RecordAsync(string blobName, long size, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            throw new InvalidOperationException("Blob name is empty.");
        }

        if (!tables.Configured)
        {
            throw new InvalidOperationException("Table storage is not configured. Set Storage:TableConnectionString.");
        }

        var recordedAt = DateTime.UtcNow.ToString("O");
        // Task (d): one audit row per upload. Status starts as Received.
        await tables.UpsertAsync(
            CloudResources.ImageAudits,
            blobName.Trim(),
            recordedAt,
            new Dictionary<string, string>
            {
                ["BlobName"] = blobName.Trim(),
                ["Size"] = size.ToString(),
                ["Status"] = "Received",
                ["RecordedAt"] = recordedAt
            },
            cancellationToken);
    }
}
