using System.Net.Http.Json;
using System.Text.Json;
using ST10538419_CLDV7112_Practicium.Application.Configuration;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using Microsoft.Extensions.Options;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Storage;

public sealed class HttpFieldFunctions(IOptions<FunctionOptions> options) : IFieldFunctions
{
    private static readonly HttpClient Http = new()
    {
        Timeout = TimeSpan.FromSeconds(8)
    };

    public bool Configured => options.Value.Configured;

    public async Task<FieldCall<IReadOnlyList<SensorReading>>> GetLatestAsync(
        string? fieldName,
        CancellationToken cancellationToken)
    {
        if (!Configured)
        {
            return new FieldCall<IReadOnlyList<SensorReading>>(
                false,
                false,
                null,
                "Telemetry is unavailable until the function URL is configured.");
        }

        // Task (d): farmer dashboard calls the HTTP GET function.
        var url = $"{options.Value.BaseUrl!.TrimEnd('/')}/api/telemetry";
        if (!string.IsNullOrWhiteSpace(fieldName))
        {
            url += $"?field={Uri.EscapeDataString(fieldName.Trim())}";
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("X-ArgiStream-Key", options.Value.SharedSecret);
        using var response = await Http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new FieldCall<IReadOnlyList<SensorReading>>(true, false, null, await ReadErrorAsync(response, cancellationToken));
        }

        var readings = await response.Content.ReadFromJsonAsync<List<SensorReading>>(FarmJson.Options, cancellationToken);
        return new FieldCall<IReadOnlyList<SensorReading>>(true, true, readings ?? [], null);
    }

    public async Task<FieldCall<string>> SendCommandAsync(
        Guid deviceId,
        string commandType,
        CancellationToken cancellationToken)
    {
        if (!Configured)
        {
            return new FieldCall<string>(false, false, null, "Irrigation is unavailable until the function URL is configured.");
        }

        // Task (d): Trigger irrigation and sensor alerts call the HTTP POST function.
        var url = $"{options.Value.BaseUrl!.TrimEnd('/')}/api/irrigation";
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(new { deviceId, commandType }, options: FarmJson.Options)
        };
        request.Headers.TryAddWithoutValidation("X-ArgiStream-Key", options.Value.SharedSecret);
        using var response = await Http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new FieldCall<string>(true, false, null, await ReadErrorAsync(response, cancellationToken));
        }

        var payload = await response.Content.ReadFromJsonAsync<CommandAccepted>(FarmJson.Options, cancellationToken);
        return new FieldCall<string>(true, true, payload?.CommandId, null);
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var payload = await response.Content.ReadFromJsonAsync<FunctionError>(FarmJson.Options, cancellationToken);
            if (!string.IsNullOrWhiteSpace(payload?.Error))
            {
                return payload.Error;
            }
        }
        catch (JsonException)
        {
        }

        return "The field function did not accept the request.";
    }

    private sealed record CommandAccepted(string? CommandId);

    private sealed record FunctionError(string? Error);
}
