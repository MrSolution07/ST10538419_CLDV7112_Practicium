using System.Text.Json;
using ST10538419_CLDV7112_Practicium.Application.Cloud;
using ST10538419_CLDV7112_Practicium.Application.Common;
using ST10538419_CLDV7112_Practicium.Application.Devices;

namespace ST10538419_CLDV7112_Practicium.Application.Farm;

public interface IIrrigationIntake
{
    Task<ServiceResult<string>> AcceptAsync(Guid deviceId, string commandType, CancellationToken cancellationToken);
}

public sealed class IrrigationIntake(IDeviceService devices, IFarmTables tables, IWorkQueue queue) : IIrrigationIntake
{
    public async Task<ServiceResult<string>> AcceptAsync(
        Guid deviceId,
        string commandType,
        CancellationToken cancellationToken)
    {
        if (!CommandTypes.IsKnown(commandType))
        {
            return ServiceResult<string>.Fail("Command type is not recognised.");
        }

        var device = await devices.GetAsync(deviceId, cancellationToken);
        if (device is null)
        {
            return ServiceResult<string>.Fail("Device was not found.");
        }

        if (!tables.Configured || !queue.Configured)
        {
            return ServiceResult<string>.Fail("Storage is not configured.");
        }

        var commandId = Guid.NewGuid().ToString("N");
        var recordedAt = DateTime.UtcNow.ToString("O");
        await tables.UpsertAsync(
            CloudResources.IrrigationCommands,
            deviceId.ToString("D"),
            commandId,
            new Dictionary<string, string>
            {
                ["CommandId"] = commandId,
                ["DeviceId"] = deviceId.ToString("D"),
                ["CommandType"] = commandType,
                ["Status"] = "Pending",
                ["RecordedAt"] = recordedAt
            },
            cancellationToken);

        var message = JsonSerializer.Serialize(new IrrigationMessage(commandId, deviceId, commandType), FarmJson.Options);
        await queue.SendAsync(CloudResources.IrrigationQueue, message, cancellationToken);
        return ServiceResult<string>.Ok(commandId);
    }
}
