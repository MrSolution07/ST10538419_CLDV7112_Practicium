using FinalCloudDev.Application.Records;
using Microsoft.Extensions.DependencyInjection;

namespace FinalCloudDev.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IReferenceRecordService, ReferenceRecordService>();
        return services;
    }
}
