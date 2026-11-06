using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Application.Devices;

public interface IDeviceRepository
{
    Task<IReadOnlyList<Device>> ListAsync(string? search, CancellationToken cancellationToken);

    Task<Device?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Device device, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
