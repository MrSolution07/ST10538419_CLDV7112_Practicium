using FinalCloudDev.Application.Configuration;
using FinalCloudDev.Application.Data;
using FinalCloudDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinalCloudDev.Infrastructure.Data;

public sealed class DatabaseStartup(
    AppDbContext db,
    IOptions<DatabaseOptions> options,
    IHostEnvironment environment,
    ILogger<DatabaseStartup> logger) : IDatabaseStartup
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (options.Value.UseSqlServer)
        {
            await db.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Azure SQL migrations applied.");
            return;
        }

        var path = ResolveSqlitePath();
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await db.Database.EnsureCreatedAsync(cancellationToken);
        if (!await db.ReferenceRecords.AnyAsync(cancellationToken))
        {
            db.ReferenceRecords.Add(new ReferenceRecord
            {
                Title = "Foundation check",
                Notes = "Sample row so the reference module is visible. Archive it once the brief has real data."
            });
            await db.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation("SQLite database ready at {Path}.", path);
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
