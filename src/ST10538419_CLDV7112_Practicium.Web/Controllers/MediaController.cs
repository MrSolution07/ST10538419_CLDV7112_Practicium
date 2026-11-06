using System.Text.RegularExpressions;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using ST10538419_CLDV7112_Practicium.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ST10538419_CLDV7112_Practicium.Web.Controllers;

[Authorize(Roles = $"{UserRoles.Farmer},{UserRoles.Agronomist}")]
public partial class MediaController(IDroneImageService images, IDroneImageCabinet cabinet) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<IActionResult> Upload(IFormFile? image, string? returnTo, CancellationToken cancellationToken)
    {
        if (image is null || image.Length == 0)
        {
            TempData["Notice"] = "Choose an image to upload.";
            return RedirectAfter(returnTo);
        }

        await using var stream = image.OpenReadStream();
        var result = await images.UploadAsync(image.FileName, stream, image.ContentType, image.Length, cancellationToken);
        TempData["Notice"] = result.Succeeded
            ? "Image uploaded. The audit function records it in table storage."
            : result.Error ?? "Could not upload the image.";
        return RedirectAfter(returnTo);
    }

    public async Task<IActionResult> Preview(string name, CancellationToken cancellationToken)
    {
        if (!cabinet.Configured || string.IsNullOrWhiteSpace(name) || !BlobName().IsMatch(name))
        {
            return NotFound();
        }

        var image = await cabinet.OpenAsync(name, cancellationToken);
        if (image is null)
        {
            return NotFound();
        }

        return File(image.Content, image.ContentType);
    }

    private IActionResult RedirectAfter(string? returnTo) =>
        returnTo == "Monitor"
            ? RedirectToAction("Index", "Monitor")
            : RedirectToAction("Index", "Dashboard");

    [GeneratedRegex("^[a-f0-9]{32}\\.(jpg|jpeg|png|webp)$", RegexOptions.IgnoreCase)]
    private static partial Regex BlobName();
}
