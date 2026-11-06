using ST10538419_CLDV7112_Practicium.Application.Cloud;

namespace ST10538419_CLDV7112_Practicium.Application.Farm;

public interface IFarmGallery
{
    Task<FarmGalleryModel> LoadAsync(CancellationToken cancellationToken);
}

public sealed class FarmGallery(IDroneImageCabinet cabinet, IFarmTables tables) : IFarmGallery
{
    public async Task<FarmGalleryModel> LoadAsync(CancellationToken cancellationToken)
    {
        if (!cabinet.Configured)
        {
            return new FarmGalleryModel(false, "Drone images are unavailable until blob storage is configured.", []);
        }

        var images = await cabinet.ListAsync(cancellationToken);
        var audits = tables.Configured
            ? await tables.ListAsync(CloudResources.ImageAudits, 100, cancellationToken)
            : [];

        var latestAudit = new Dictionary<string, FarmRow>(StringComparer.OrdinalIgnoreCase);
        foreach (var audit in audits)
        {
            var name = audit.Values.GetValueOrDefault("BlobName") ?? audit.PartitionKey;
            if (!latestAudit.TryGetValue(name, out var current) ||
                string.Compare(audit.Values.GetValueOrDefault("RecordedAt"), current.Values.GetValueOrDefault("RecordedAt"), StringComparison.Ordinal) > 0)
            {
                latestAudit[name] = audit;
            }
        }

        var gallery = images
            .OrderBy(image => image.Name)
            .Select(image =>
            {
                latestAudit.TryGetValue(image.Name, out var audit);
                return new GalleryImage(
                    image.Name,
                    audit?.Values.GetValueOrDefault("Status") ?? "Waiting for audit",
                    audit?.Values.GetValueOrDefault("RecordedAt") ?? "");
            })
            .ToList();

        return new FarmGalleryModel(true, null, gallery);
    }
}
