using FinalCloudDev.Application.Configuration;
using FinalCloudDev.Application.Records;
using FinalCloudDev.Web.Models;
using FinalCloudDev.Web.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FinalCloudDev.Web.Controllers;

public class HomeController(
    IReferenceRecordService records,
    IOptions<SiteOptions> site,
    IOptions<DatabaseOptions> database,
    IOptions<StorageOptions> storage) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new HomeStatusViewModel
        {
            SiteName = site.Value.Name,
            Tagline = site.Value.Tagline,
            DatabaseProvider = database.Value.Provider,
            ActiveRecords = await records.CountActiveAsync(cancellationToken),
            BlobConfigured = storage.Value.BlobConfigured,
            QueueConfigured = storage.Value.QueueConfigured,
            TableConfigured = storage.Value.TableConfigured,
            FileShareConfigured = storage.Value.FileShareConfigured
        };

        return View(model);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
