using ST10538419_CLDV7112_Practicium.Application.Configuration;
using ST10538419_CLDV7112_Practicium.Web.Models;
using Microsoft.Extensions.Options;

namespace ST10538419_CLDV7112_Practicium.Web;

public sealed class HostingStatusReader(
    IOptions<DatabaseOptions> database,
    IOptions<StorageOptions> storage,
    IConfiguration configuration)
{
    public HostingStatusViewModel Read(HttpRequest request) => new()
    {
        Host = request.Host.Value ?? "local",
        DatabaseProvider = database.Value.Provider,
        BlobReady = storage.Value.BlobConfigured,
        QueueReady = storage.Value.QueueConfigured,
        TableReady = storage.Value.TableConfigured,
        LogsReady = !string.IsNullOrWhiteSpace(configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"])
    };
}
