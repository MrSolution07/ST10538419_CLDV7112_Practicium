using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST10538419_CLDV7112_Practicium.Application.Farm;

namespace ST10538419_CLDV7112_Practicium.Functions;

public sealed class GetTelemetryFunction(IServiceScopeFactory scopes, IConfiguration configuration)
{
    [Function(nameof(GetTelemetryFunction))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "telemetry")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        if (!FunctionResponses.Authorized(request, configuration))
        {
            return await FunctionResponses.Json(request, HttpStatusCode.Unauthorized, new { error = "The function key was rejected." });
        }

        var field = FunctionResponses.QueryValue(request.Url, "field");
        await using var scope = scopes.CreateAsyncScope();
        var telemetry = scope.ServiceProvider.GetRequiredService<ITelemetryQuery>();
        var readings = await telemetry.LatestAsync(field, cancellationToken);
        return await FunctionResponses.Json(request, HttpStatusCode.OK, readings);
    }
}
