using ST10538419_CLDV7112_Practicium.Application.Cloud;
using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using ST10538419_CLDV7112_Practicium.Domain;

namespace ST10538419_CLDV7112_Practicium.Tests;

public class IrrigationIntakeTests
{
    private readonly DeviceService devices = new(new DeviceServiceTestsRepository());
    private readonly RecordingTables tables = new();
    private readonly RecordingQueue queue = new();

    [Fact]
    public async Task Accept_rejects_an_unknown_command()
    {
        var intake = new IrrigationIntake(devices, tables, queue);

        var result = await intake.AcceptAsync(Guid.NewGuid(), "flood", CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Command type is not recognised.", result.Error);
        Assert.Empty(tables.Writes);
        Assert.Empty(queue.Messages);
    }

    [Fact]
    public async Task Accept_writes_the_table_before_the_queue()
    {
        var created = await devices.CreateAsync(new DeviceInput("North pivot", "North", DeviceStatuses.Idle), CancellationToken.None);
        var intake = new IrrigationIntake(devices, tables, queue);

        var result = await intake.AcceptAsync(created.Value!.Id, CommandTypes.Irrigate, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal(CloudResources.IrrigationCommands, tables.Writes[0].Table);
        Assert.Equal("Pending", tables.Writes[0].Values["Status"]);
        Assert.Single(queue.Messages);
        Assert.Contains(result.Value, queue.Messages[0]);
    }

    [Fact]
    public async Task Processor_sets_needs_attention_for_a_sensor_alert()
    {
        var created = await devices.CreateAsync(new DeviceInput("North pivot", "North", DeviceStatuses.Idle), CancellationToken.None);
        var intake = new IrrigationIntake(devices, tables, queue);
        await intake.AcceptAsync(created.Value!.Id, CommandTypes.SensorAlert, CancellationToken.None);
        var processor = new IrrigationProcessor(devices, tables);

        var outcome = await processor.ProcessAsync(queue.Messages[0], CancellationToken.None);
        var device = await devices.GetAsync(created.Value.Id, CancellationToken.None);

        Assert.Contains(DeviceStatuses.NeedsAttention, outcome);
        Assert.Equal(DeviceStatuses.NeedsAttention, device!.OperationalStatus);
        Assert.Contains(tables.Writes, write => write.Values["Status"] == "Processed");
    }

    private sealed class DeviceServiceTestsRepository : IDeviceRepository
    {
        private readonly List<ST10538419_CLDV7112_Practicium.Domain.Entities.Device> devices = [];

        public Task<IReadOnlyList<ST10538419_CLDV7112_Practicium.Domain.Entities.Device>> ListAsync(string? search, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<ST10538419_CLDV7112_Practicium.Domain.Entities.Device>>(devices);

        public Task<ST10538419_CLDV7112_Practicium.Domain.Entities.Device?> GetAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(devices.FirstOrDefault(device => device.Id == id));

        public Task AddAsync(ST10538419_CLDV7112_Practicium.Domain.Entities.Device device, CancellationToken cancellationToken)
        {
            devices.Add(device);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class RecordingTables : IFarmTables
    {
        public List<(string Table, Dictionary<string, string> Values)> Writes { get; } = [];

        public bool Configured => true;

        public Task UpsertAsync(
            string tableName,
            string partitionKey,
            string rowKey,
            IReadOnlyDictionary<string, string> values,
            CancellationToken cancellationToken)
        {
            Writes.Add((tableName, new Dictionary<string, string>(values)));
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<FarmRow>> ListAsync(string tableName, int take, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<FarmRow>>([]);
    }

    private sealed class RecordingQueue : IWorkQueue
    {
        public List<string> Messages { get; } = [];

        public bool Configured => true;

        public Task SendAsync(string queueName, string message, CancellationToken cancellationToken)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
