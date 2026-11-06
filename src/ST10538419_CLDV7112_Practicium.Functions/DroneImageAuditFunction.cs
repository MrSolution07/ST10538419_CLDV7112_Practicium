using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ST10538419_CLDV7112_Practicium.Application.Farm;

namespace ST10538419_CLDV7112_Practicium.Functions;

public sealed class DroneImageAuditFunction(IServiceScopeFactory scopes, ILogger<DroneImageAuditFunction> logger)
{
    [Function(nameof(DroneImageAuditFunction))]
    public async Task Run(
        [BlobTrigger("drone-images/{name}", Connection = "AzureWebJobsStorage")] Stream blob,
        string name,
        CancellationToken cancellationToken)
    {
        var size = blob.CanSeek ? blob.Length : 0;
        logger.LogInformation("Drone image {Name} uploaded. Size {Size} bytes.", name, size);

        await using var scope = scopes.CreateAsyncScope();
        var audits = scope.ServiceProvider.GetRequiredService<IImageAuditProcessor>();
        await audits.RecordAsync(name, size, cancellationToken);
    }
}
