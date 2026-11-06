using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST10538419_CLDV7112_Practicium.Application.Farm;

namespace ST10538419_CLDV7112_Practicium.Functions;

public sealed class PostIrrigationFunction(IServiceScopeFactory scopes, IConfiguration configuration)
{
    [Function(nameof(PostIrrigationFunction))]
    // Task (d): HTTP POST /api/irrigation. IrrigationIntake stores the command and places it on the queue.
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "irrigation")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        if (!FunctionResponses.Authorized(request, configuration))
        {
            return await FunctionResponses.Json(request, HttpStatusCode.Unauthorized, new { error = "The function key was rejected." });
        }

        IrrigationBody? body;
        try
        {
            using var reader = new StreamReader(request.Body);
            var json = await reader.ReadToEndAsync(cancellationToken);
            body = JsonSerializer.Deserialize<IrrigationBody>(json, FarmJson.Options);
        }
        catch (JsonException)
        {
            body = null;
        }

        if (body is null || body.DeviceId == Guid.Empty || string.IsNullOrWhiteSpace(body.CommandType))
        {
            return await FunctionResponses.Json(request, HttpStatusCode.BadRequest, new { error = "Device and command type are required." });
        }

        await using var scope = scopes.CreateAsyncScope();
        var intake = scope.ServiceProvider.GetRequiredService<IIrrigationIntake>();
        var result = await intake.AcceptAsync(body.DeviceId, body.CommandType, cancellationToken);
        if (!result.Succeeded)
        {
            return await FunctionResponses.Json(request, HttpStatusCode.BadRequest, new { error = result.Error });
        }

        return await FunctionResponses.Json(request, HttpStatusCode.OK, new { commandId = result.Value });
    }

    private sealed record IrrigationBody(Guid DeviceId, string? CommandType);
}
