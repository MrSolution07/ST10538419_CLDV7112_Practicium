using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ST10538419_CLDV7112_Practicium.Web.Controllers;

[Authorize]
public class DevicesController(IDeviceService devices) : Controller
{
    public async Task<IActionResult> Index(string? q, CancellationToken cancellationToken)
    {
        ViewData["Search"] = q;
        return View(await devices.ListAsync(q, cancellationToken));
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var device = await devices.GetAsync(id, cancellationToken);
        return device is null ? NotFound() : View(device);
    }

    [Authorize(Roles = UserRoles.Admin)]
    public IActionResult Create() => View(new DeviceForm());

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DeviceForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var result = await devices.CreateAsync(form.ToInput(), cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Could not save the device.");
            return View(form);
        }

        return RedirectToAction(nameof(Details), new { id = result.Value!.Id });
    }

    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var device = await devices.GetAsync(id, cancellationToken);
        return device is null ? NotFound() : View(DeviceForm.FromDevice(device));
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, DeviceForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var result = await devices.UpdateAsync(id, form.ToInput(), cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Could not update the device.");
            return View(form);
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}
