using ST10538419_CLDV7112_Practicium.Application.Accounts;
using ST10538419_CLDV7112_Practicium.Application.Configuration;
using ST10538419_CLDV7112_Practicium.Application.Data;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Data;

public sealed class DatabaseStartup(
    AppDbContext db,
    IOptions<DatabaseOptions> options,
    IHostEnvironment environment,
    ILogger<DatabaseStartup> logger) : IDatabaseStartup
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Task (b): production applies the EF migrations to Azure SQL. Development keeps using SQLite.
        if (options.Value.UseSqlServer)
        {
            await db.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Azure SQL migrations applied.");
        }
        else
        {
            var path = ResolveSqlitePath();
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await db.Database.EnsureCreatedAsync(cancellationToken);
            logger.LogInformation("SQLite database ready at {Path}.", path);
        }

        await SeedFarmAsync(cancellationToken);
    }

    private async Task SeedFarmAsync(CancellationToken cancellationToken)
    {
        foreach (var demo in DemoAccounts.All)
        {
            var exists = await db.UserAccounts.AnyAsync(
                account => account.Username == demo.Username,
                cancellationToken);
            if (exists)
            {
                continue;
            }

            db.UserAccounts.Add(new UserAccount
            {
                Username = demo.Username,
                PasswordHash = PasswordHasher.Hash(demo.Password),
                Role = demo.Role
            });
        }

        if (!await db.Devices.AnyAsync(cancellationToken))
        {
            db.Devices.Add(new Device
            {
                Name = "North pivot",
                FieldName = "North",
                OperationalStatus = DeviceStatuses.Idle
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private string ResolveSqlitePath()
    {
        var path = options.Value.SqlitePath;
        if (Path.IsPathRooted(path))
        {
            return path;
        }

        return Path.Combine(environment.ContentRootPath, path);
    }
}
