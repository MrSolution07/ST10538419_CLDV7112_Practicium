using ST10538419_CLDV7112_Practicium.Application.Common;

namespace ST10538419_CLDV7112_Practicium.Application.Farm;

public interface IDroneImageService
{
    Task<ServiceResult<string>> UploadAsync(
        string fileName,
        Stream content,
        string contentType,
        long length,
        CancellationToken cancellationToken);
}

public sealed class DroneImageService(IDroneImageCabinet cabinet) : IDroneImageService
{
    public const long MaxBytes = 8 * 1024 * 1024;

    public async Task<ServiceResult<string>> UploadAsync(
        string fileName,
        Stream content,
        string contentType,
        long length,
        CancellationToken cancellationToken)
    {
        if (!cabinet.Configured)
        {
            return ServiceResult<string>.Fail("Blob storage is not configured.");
        }

        if (length <= 0 || length > MaxBytes)
        {
            return ServiceResult<string>.Fail("Image must be between 1 byte and 8 MB.");
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (extension is not ".jpg" and not ".jpeg" and not ".png" and not ".webp")
        {
            return ServiceResult<string>.Fail("Use a JPG, PNG, or WebP image.");
        }

        var blobName = $"{Guid.NewGuid():N}{extension}";
        var type = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType;
        await cabinet.UploadAsync(blobName, content, type, cancellationToken);
        return ServiceResult<string>.Ok(blobName);
    }
}
