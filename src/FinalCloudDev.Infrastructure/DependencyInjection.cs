using FinalCloudDev.Application.Configuration;
using FinalCloudDev.Application.Data;
using FinalCloudDev.Application.Records;
using FinalCloudDev.Infrastructure.Data;
using FinalCloudDev.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinalCloudDev.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));

        var database = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();
        var azureSql = configuration.GetConnectionString("AzureSql");
        if (environment.IsProduction() && !database.UseSqlServer)
        {
            throw new InvalidOperationException("Production must set Database__Provider=SqlServer.");
        }

        if (database.UseSqlServer && string.IsNullOrWhiteSpace(azureSql))
        {
            throw new InvalidOperationException("ConnectionStrings:AzureSql is empty.");
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            if (database.UseSqlServer)
            {
                options.UseSqlServer(azureSql, sql =>
                {
                    sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    sql.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
                });
                return;
            }

            var path = database.SqlitePath;
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(environment.ContentRootPath, path);
            }

            options.UseSqlite($"Data Source={path}");
        });

        services.AddScoped<IReferenceRecordRepository, ReferenceRecordRepository>();
        services.AddScoped<IDatabaseStartup, DatabaseStartup>();

        var storage = configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>() ?? new StorageOptions();
        Register<IBlobStore, AzureBlobStore, UnconfiguredBlobStore>(services, storage.BlobConfigured);
        Register<IQueueStore, AzureQueueStore, UnconfiguredQueueStore>(services, storage.QueueConfigured);
        Register<ITableStore, AzureTableStore, UnconfiguredTableStore>(services, storage.TableConfigured);
        Register<IFileShareStore, AzureFileShareStore, UnconfiguredFileShareStore>(services, storage.FileShareConfigured);

        return services;
    }

    private static void Register<TService, TReal, TMissing>(IServiceCollection services, bool configured)
        where TService : class
        where TReal : class, TService
        where TMissing : class, TService
    {
        if (configured)
        {
            services.AddSingleton<TService, TReal>();
            return;
        }

        services.AddSingleton<TService, TMissing>();
    }
}
