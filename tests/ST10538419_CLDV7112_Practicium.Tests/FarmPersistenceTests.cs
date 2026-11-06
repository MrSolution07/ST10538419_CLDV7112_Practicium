using ST10538419_CLDV7112_Practicium.Application.Accounts;
using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ST10538419_CLDV7112_Practicium.Tests;

public class FarmPersistenceTests
{
    [Fact]
    public async Task Sqlite_round_trip_saves_a_user_account()
    {
        await using var db = await OpenAsync();
        var service = new UserAccountService(new UserAccountRepository(db));

        var created = await service.CreateAsync(
            new UserAccountInput("farmer", "Farmer-field-1", UserRoles.Farmer),
            CancellationToken.None);
        var signedIn = await service.SignInAsync("farmer", "Farmer-field-1", CancellationToken.None);

        Assert.True(created.Succeeded);
        Assert.True(signedIn.Succeeded);
        Assert.Equal(created.Value!.Id, signedIn.Value!.Id);
    }

    [Fact]
    public async Task Sqlite_round_trip_saves_and_searches_a_device()
    {
        await using var db = await OpenAsync();
        var service = new DeviceService(new DeviceRepository(db));

        var created = await service.CreateAsync(
            new DeviceInput("North pivot", "North", DeviceStatuses.Idle),
            CancellationToken.None);
        var listed = await service.ListAsync("North", CancellationToken.None);

        Assert.True(created.Succeeded);
        Assert.Single(listed);
        Assert.Equal("North pivot", listed[0].Name);
    }

    private static async Task<AppDbContext> OpenAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();
        return db;
    }
}
