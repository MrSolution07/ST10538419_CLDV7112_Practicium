using FinalCloudDev.Application.Records;
using FinalCloudDev.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FinalCloudDev.Tests;

public class ReferenceRecordPersistenceTests
{
    [Fact]
    public async Task Sqlite_round_trip_saves_and_searches()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var service = new ReferenceRecordService(new ReferenceRecordRepository(db));
        var created = await service.CreateAsync(new ReferenceRecordInput("Lab note", "first"), CancellationToken.None);
        var listed = await service.ListAsync("Lab", CancellationToken.None);

        Assert.True(created.Succeeded);
        Assert.Single(listed);
        Assert.Equal("Lab note", listed[0].Title);
    }
}
