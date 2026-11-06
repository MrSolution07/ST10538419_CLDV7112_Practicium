using ST10538419_CLDV7112_Practicium.Application.Accounts;
using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ST10538419_CLDV7112_Practicium.Web.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class OperationsController(
    IUserAccountService accounts,
    IDeviceService devices,
    HostingStatusReader hosting) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new OperationsViewModel
        {
            Hosting = hosting.Read(Request),
            AccountCount = await accounts.CountAsync(cancellationToken),
            DeviceCount = await devices.CountAsync(cancellationToken)
        };
        return View(model);
    }
}
