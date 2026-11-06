using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using ST10538419_CLDV7112_Practicium.Application;
using ST10538419_CLDV7112_Practicium.Infrastructure;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplication();
        services.AddInfrastructure(context.Configuration, context.HostingEnvironment);
    })
    .Build();

host.Run();
