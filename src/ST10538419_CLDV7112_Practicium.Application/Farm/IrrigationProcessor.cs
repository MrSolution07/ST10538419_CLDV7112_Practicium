using System.Text.Json;
using ST10538419_CLDV7112_Practicium.Application.Cloud;
using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Domain;

namespace ST10538419_CLDV7112_Practicium.Application.Farm;

public interface IIrrigationProcessor
{
    Task<string> ProcessAsync(string message, CancellationToken cancellationToken);
}

public sealed class IrrigationProcessor(IDeviceService devices, IFarmTables tables) : IIrrigationProcessor
{
    public async Task<string> ProcessAsync(string message, CancellationToken cancellationToken)
    {
        IrrigationMessage? payload;
        try
        {
            payload = JsonSerializer.Deserialize<IrrigationMessage>(message, FarmJson.Options);
        }
        catch (JsonException)
        {
            return "Irrigation message was not valid JSON.";
        }

        if (payload is null || payload.DeviceId == Guid.Empty || string.IsNullOrWhiteSpace(payload.CommandId))
        {
            return "Irrigation message was missing a device.";
        }

        var status = payload.CommandType == CommandTypes.SensorAlert
            ? DeviceStatuses.NeedsAttention
            : DeviceStatuses.Irrigating;

        var updated = await devices.SetStatusAsync(payload.DeviceId, status, cancellationToken);
        if (!updated.Succeeded)
        {
            return updated.Error ?? "Device was not found.";
        }

        if (tables.Configured)
        {
            await tables.UpsertAsync(
                CloudResources.IrrigationCommands,
                payload.DeviceId.ToString("D"),
                payload.CommandId,
                new Dictionary<string, string>
                {
                    ["CommandId"] = payload.CommandId,
                    ["DeviceId"] = payload.DeviceId.ToString("D"),
                    ["CommandType"] = payload.CommandType,
                    ["Status"] = "Processed",
                    ["RecordedAt"] = DateTime.UtcNow.ToString("O")
                },
                cancellationToken);
        }

        return $"Device {payload.DeviceId:D} is {status}.";
    }
}
