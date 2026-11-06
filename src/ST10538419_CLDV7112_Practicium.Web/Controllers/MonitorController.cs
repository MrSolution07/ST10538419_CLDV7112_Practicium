using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ST10538419_CLDV7112_Practicium.Web.Controllers;

[Authorize(Roles = UserRoles.Agronomist)]
public class MonitorController(
    IDeviceService devices,
    IEnvironmentalLogQuery logs,
    IFarmGallery gallery,
    HostingStatusReader hosting) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new MonitorViewModel
        {
            Hosting = hosting.Read(Request),
            Devices = await devices.ListAsync(null, cancellationToken),
            Logs = await logs.LatestAsync(cancellationToken),
            Gallery = await gallery.LoadAsync(cancellationToken)
        };
        return View(model);
    }
}
