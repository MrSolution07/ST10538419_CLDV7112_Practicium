using ST10538419_CLDV7112_Practicium.Application.Common;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Application.Devices;

public interface IDeviceService
{
    Task<IReadOnlyList<Device>> ListAsync(string? search, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<Device?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<ServiceResult<Device>> CreateAsync(DeviceInput input, CancellationToken cancellationToken);

    Task<ServiceResult<Device>> UpdateAsync(Guid id, DeviceInput input, CancellationToken cancellationToken);

    Task<ServiceResult<Device>> SetStatusAsync(Guid id, string status, CancellationToken cancellationToken);
}
