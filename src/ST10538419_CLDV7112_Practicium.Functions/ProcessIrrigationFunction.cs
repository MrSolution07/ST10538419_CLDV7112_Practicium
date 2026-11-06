using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ST10538419_CLDV7112_Practicium.Application.Cloud;
using ST10538419_CLDV7112_Practicium.Application.Farm;

namespace ST10538419_CLDV7112_Practicium.Functions;

public sealed class ProcessIrrigationFunction(IServiceScopeFactory scopes, ILogger<ProcessIrrigationFunction> logger)
{
    [Function(nameof(ProcessIrrigationFunction))]
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
