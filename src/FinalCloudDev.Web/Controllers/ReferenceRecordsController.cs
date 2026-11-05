using ST10538419_CLDV7112_Practicium.Application.Records;
using ST10538419_CLDV7112_Practicium.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ST10538419_CLDV7112_Practicium.Web.Controllers;

public class ReferenceRecordsController(IReferenceRecordService records) : Controller
{
    public async Task<IActionResult> Index(string? q, CancellationToken cancellationToken)
    {
        ViewData["Search"] = q;
        var items = await records.ListAsync(q, cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var record = await records.GetAsync(id, cancellationToken);
        if (record is null || record.IsArchived)
        {
            return NotFound();
        }

        return View(record);
    }

    public IActionResult Create() => View(new ReferenceRecordForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReferenceRecordForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var result = await records.CreateAsync(form.ToInput(), cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Could not save the record.");
            return View(form);
        }

        return RedirectToAction(nameof(Details), new { id = result.Value!.Id });
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var record = await records.GetAsync(id, cancellationToken);
        if (record is null || record.IsArchived)
        {
            return NotFound();
        }

        return View(ReferenceRecordForm.FromRecord(record.Id, record.Title, record.Notes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ReferenceRecordForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var result = await records.UpdateAsync(id, form.ToInput(), cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Could not update the record.");
            return View(form);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(Guid id, CancellationToken cancellationToken)
    {
        await records.ArchiveAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
