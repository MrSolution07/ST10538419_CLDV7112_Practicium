using ST10538419_CLDV7112_Practicium.Application.Common;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Application.Devices;

public sealed class DeviceService(IDeviceRepository repository) : IDeviceService
{
    public const int NameMaxLength = 80;

    public const int FieldMaxLength = 80;

    public Task<IReadOnlyList<Device>> ListAsync(string? search, CancellationToken cancellationToken) =>
        repository.ListAsync(search, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken) =>
        (await repository.ListAsync(null, cancellationToken)).Count;

    public Task<Device?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        repository.GetAsync(id, cancellationToken);

    public async Task<ServiceResult<Device>> CreateAsync(DeviceInput input, CancellationToken cancellationToken)
    {
        var error = Validate(input);
        if (error is not null)
        {
            return ServiceResult<Device>.Fail(error);
        }

        var device = new Device
        {
            Name = input.Name.Trim(),
            FieldName = input.FieldName.Trim(),
            OperationalStatus = input.OperationalStatus.Trim()
        };

        await repository.AddAsync(device, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return ServiceResult<Device>.Ok(device);
    }

    public async Task<ServiceResult<Device>> UpdateAsync(Guid id, DeviceInput input, CancellationToken cancellationToken)
    {
        var error = Validate(input);
        if (error is not null)
        {
            return ServiceResult<Device>.Fail(error);
        }

        var device = await repository.GetAsync(id, cancellationToken);
        if (device is null)
        {
            return ServiceResult<Device>.Fail("Device was not found.");
        }

        device.Name = input.Name.Trim();
        device.FieldName = input.FieldName.Trim();
        device.OperationalStatus = input.OperationalStatus.Trim();
        device.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return ServiceResult<Device>.Ok(device);
    }

    public async Task<ServiceResult<Device>> SetStatusAsync(Guid id, string status, CancellationToken cancellationToken)
    {
        if (!DeviceStatuses.IsKnown(status))
        {
            return ServiceResult<Device>.Fail("Status is not recognised.");
        }

        var device = await repository.GetAsync(id, cancellationToken);
        if (device is null)
        {
            return ServiceResult<Device>.Fail("Device was not found.");
        }

        device.OperationalStatus = status.Trim();
        device.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return ServiceResult<Device>.Ok(device);
    }

    private static string? Validate(DeviceInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            return "Name is required.";
        }

        if (input.Name.Trim().Length > NameMaxLength)
        {
            return $"Name must be {NameMaxLength} characters or fewer.";
        }

        if (string.IsNullOrWhiteSpace(input.FieldName))
        {
            return "Field name is required.";
        }

        if (input.FieldName.Trim().Length > FieldMaxLength)
        {
            return $"Field name must be {FieldMaxLength} characters or fewer.";
        }

        if (!DeviceStatuses.IsKnown(input.OperationalStatus))
        {
            return "Status is not recognised.";
        }

        return null;
    }
}
