using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Data;

public sealed class DeviceRepository(AppDbContext db) : IDeviceRepository
{
    public async Task<IReadOnlyList<Device>> ListAsync(string? search, CancellationToken cancellationToken)
    {
        var query = db.Devices.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(device => device.Name.Contains(term) || device.FieldName.Contains(term));
        }

        return await query
            .OrderBy(device => device.FieldName)
            .ThenBy(device => device.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Device?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        db.Devices.FirstOrDefaultAsync(device => device.Id == id, cancellationToken);

    public async Task AddAsync(Device device, CancellationToken cancellationToken) =>
        await db.Devices.AddAsync(device, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        db.SaveChangesAsync(cancellationToken);
}
