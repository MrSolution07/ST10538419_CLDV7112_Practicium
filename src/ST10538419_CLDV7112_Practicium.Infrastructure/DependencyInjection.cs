using ST10538419_CLDV7112_Practicium.Application.Accounts;
using ST10538419_CLDV7112_Practicium.Application.Configuration;
using ST10538419_CLDV7112_Practicium.Application.Data;
using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using ST10538419_CLDV7112_Practicium.Infrastructure.Data;
using ST10538419_CLDV7112_Practicium.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ST10538419_CLDV7112_Practicium.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
        services.Configure<FunctionOptions>(configuration.GetSection(FunctionOptions.SectionName));

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

        services.AddScoped<IUserAccountRepository, UserAccountRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IDatabaseStartup, DatabaseStartup>();
        services.AddSingleton<IFarmTables, FarmTableGateway>();
        services.AddSingleton<IWorkQueue, StorageWorkQueue>();
        services.AddSingleton<IDroneImageCabinet, DroneImageCabinet>();
        services.AddSingleton<IFieldFunctions, HttpFieldFunctions>();
        services.AddSingleton<ICloudBootstrap, CloudBootstrap>();

        var storage = configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>() ?? new StorageOptions();
        if (environment.IsProduction() &&
            (!storage.BlobConfigured || !storage.QueueConfigured || !storage.TableConfigured || !storage.FileShareConfigured))
        {
            throw new InvalidOperationException(
                "Production storage connection strings are empty. Blob, queue, table, and file share must be set.");
        }

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
