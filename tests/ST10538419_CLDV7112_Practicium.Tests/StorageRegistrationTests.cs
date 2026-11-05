using ST10538419_CLDV7112_Practicium.Infrastructure;
using ST10538419_CLDV7112_Practicium.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace ST10538419_CLDV7112_Practicium.Tests;

public class StorageRegistrationTests
{
    [Fact]
    public void Empty_connection_strings_register_unconfigured_stores()
    {
        using var provider = BuildProvider(new Dictionary<string, string?>
        {
            ["Database:Provider"] = "Sqlite",
            ["Storage:BlobConnectionString"] = "",
            ["Storage:QueueConnectionString"] = "",
            ["Storage:TableConnectionString"] = "",
            ["Storage:FileShareConnectionString"] = ""
        });

        Assert.IsType<UnconfiguredBlobStore>(provider.GetRequiredService<IBlobStore>());
        Assert.IsType<UnconfiguredQueueStore>(provider.GetRequiredService<IQueueStore>());
        Assert.IsType<UnconfiguredTableStore>(provider.GetRequiredService<ITableStore>());
        Assert.IsType<UnconfiguredFileShareStore>(provider.GetRequiredService<IFileShareStore>());
    }

    [Fact]
    public void Blob_connection_string_registers_azure_store()
    {
        using var provider = BuildProvider(new Dictionary<string, string?>
        {
            ["Database:Provider"] = "Sqlite",
            ["Storage:BlobConnectionString"] = "UseDevelopmentStorage=true"
        });

        Assert.IsType<AzureBlobStore>(provider.GetRequiredService<IBlobStore>());
    }

    private static ServiceProvider BuildProvider(Dictionary<string, string?> values)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(configuration, new TestEnvironment());
        return services.BuildServiceProvider();
    }

    private sealed class TestEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;

        public string ApplicationName { get; set; } = "ST10538419_CLDV7112_Practicium.Tests";

        public string ContentRootPath { get; set; } = Path.GetTempPath();

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
