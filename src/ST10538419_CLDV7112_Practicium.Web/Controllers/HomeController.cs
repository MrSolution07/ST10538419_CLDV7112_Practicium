using ST10538419_CLDV7112_Practicium.Application.Configuration;
using ST10538419_CLDV7112_Practicium.Application.Records;
using ST10538419_CLDV7112_Practicium.Web.Models;
using ST10538419_CLDV7112_Practicium.Web.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ST10538419_CLDV7112_Practicium.Web.Controllers;

public class HomeController(
    IReferenceRecordService records,
    IOptions<SiteOptions> site,
    IOptions<DatabaseOptions> database,
    IOptions<StorageOptions> storage,
    IConfiguration configuration) : Controller
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
            FileShareConfigured = storage.Value.FileShareConfigured,
            LogsConfigured = !string.IsNullOrWhiteSpace(configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"])
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
