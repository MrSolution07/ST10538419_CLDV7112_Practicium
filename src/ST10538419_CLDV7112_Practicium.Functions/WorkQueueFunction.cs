using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ST10538419_CLDV7112_Practicium.Application.Cloud;

namespace ST10538419_CLDV7112_Practicium.Functions;

public class WorkQueueFunction(ILogger<WorkQueueFunction> logger)
{
    [Function(nameof(WorkQueueFunction))]
    public void Run([QueueTrigger(CloudResources.WorkQueue)] string message)
    {
        logger.LogInformation("Queue {Queue} received: {Message}", CloudResources.WorkQueue, message);
    }
}
