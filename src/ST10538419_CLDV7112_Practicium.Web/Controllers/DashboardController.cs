using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ST10538419_CLDV7112_Practicium.Web.Controllers;

[Authorize(Roles = UserRoles.Farmer)]
public class DashboardController(
    IDeviceService devices,
    IFieldFunctions functions,
    IFarmGallery gallery,
    HostingStatusReader hosting) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var readings = await functions.GetLatestAsync(null, cancellationToken);
        var model = new FarmerDashboardViewModel
        {
            Hosting = hosting.Read(Request),
            Devices = await devices.ListAsync(null, cancellationToken),
            Readings = readings.Value ?? [],
            Gallery = await gallery.LoadAsync(cancellationToken),
            TelemetryMessage = readings.Succeeded ? null : readings.Message
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Irrigate(Guid deviceId, CancellationToken cancellationToken)
    {
        var result = await functions.SendCommandAsync(deviceId, CommandTypes.Irrigate, cancellationToken);
        TempData["Notice"] = result.Succeeded
            ? "Irrigation command sent."
            : result.Message ?? "Could not send the irrigation command.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Report(Guid deviceId, CancellationToken cancellationToken)
    {
        var result = await functions.SendCommandAsync(deviceId, CommandTypes.SensorAlert, cancellationToken);
        TempData["Notice"] = result.Succeeded
            ? "Sensor problem sent."
            : result.Message ?? "Could not send the sensor problem.";
        return RedirectToAction(nameof(Index));
    }
}
