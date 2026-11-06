using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Tests;

public class DeviceServiceTests
{
    private readonly InMemoryDeviceRepository repository = new();
    private readonly DeviceService service;

    public DeviceServiceTests()
    {
        service = new DeviceService(repository);
    }

    [Fact]
    public async Task Create_rejects_blank_name()
    {
        var result = await service.CreateAsync(new DeviceInput("  ", "North", DeviceStatuses.Idle), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Name is required.", result.Error);
    }

    [Fact]
    public async Task Create_rejects_unknown_status()
    {
        var result = await service.CreateAsync(new DeviceInput("North pivot", "North", "Broken"), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Status is not recognised.", result.Error);
    }

    [Fact]
    public async Task Set_status_updates_an_existing_device()
    {
        var created = await service.CreateAsync(new DeviceInput("North pivot", "North", DeviceStatuses.Idle), CancellationToken.None);

        var updated = await service.SetStatusAsync(created.Value!.Id, DeviceStatuses.Irrigating, CancellationToken.None);

        Assert.True(updated.Succeeded);
        Assert.Equal(DeviceStatuses.Irrigating, updated.Value!.OperationalStatus);
    }

    [Fact]
    public async Task Set_status_missing_device_fails()
    {
        var result = await service.SetStatusAsync(Guid.NewGuid(), DeviceStatuses.Idle, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Device was not found.", result.Error);
    }

    private sealed class InMemoryDeviceRepository : IDeviceRepository
    {
        private readonly List<Device> devices = [];

        public Task<IReadOnlyList<Device>> ListAsync(string? search, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Device>>(devices);

        public Task<Device?> GetAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(devices.FirstOrDefault(device => device.Id == id));

        public Task AddAsync(Device device, CancellationToken cancellationToken)
        {
            devices.Add(device);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
