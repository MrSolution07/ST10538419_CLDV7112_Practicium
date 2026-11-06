using ST10538419_CLDV7112_Practicium.Application.Accounts;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ST10538419_CLDV7112_Practicium.Web.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class UsersController(IUserAccountService accounts) : Controller
{
    public async Task<IActionResult> Index(string? q, CancellationToken cancellationToken)
    {
        ViewData["Search"] = q;
        return View(await accounts.ListAsync(q, cancellationToken));
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var account = await accounts.GetAsync(id, cancellationToken);
        return account is null ? NotFound() : View(account);
    }

    public IActionResult Create() => View(new UserAccountForm { Role = UserRoles.Farmer });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserAccountForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var result = await accounts.CreateAsync(form.ToInput(), cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Could not save the account.");
            return View(form);
        }

        return RedirectToAction(nameof(Details), new { id = result.Value!.Id });
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var account = await accounts.GetAsync(id, cancellationToken);
        return account is null ? NotFound() : View(UserAccountForm.FromAccount(account));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UserAccountForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var result = await accounts.UpdateAsync(id, form.ToInput(), cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Could not update the account.");
            return View(form);
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}
