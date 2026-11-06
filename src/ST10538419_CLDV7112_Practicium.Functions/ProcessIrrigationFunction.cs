using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ST10538419_CLDV7112_Practicium.Application.Cloud;
using ST10538419_CLDV7112_Practicium.Application.Farm;

namespace ST10538419_CLDV7112_Practicium.Functions;

public sealed class ProcessIrrigationFunction(IServiceScopeFactory scopes, ILogger<ProcessIrrigationFunction> logger)
{
    [Function(nameof(ProcessIrrigationFunction))]
    // Task (d): queue trigger on irrigation-commands. IrrigationProcessor updates the device row in Azure SQL.
    public async Task Run(
        [QueueTrigger(CloudResources.IrrigationQueue, Connection = "AzureWebJobsStorage")] string message,
        CancellationToken cancellationToken)
    {
        await using var scope = scopes.CreateAsyncScope();
        var processor = scope.ServiceProvider.GetRequiredService<IIrrigationProcessor>();
        var outcome = await processor.ProcessAsync(message, cancellationToken);
        logger.LogInformation("Irrigation queue result: {Outcome}", outcome);
    }
}
