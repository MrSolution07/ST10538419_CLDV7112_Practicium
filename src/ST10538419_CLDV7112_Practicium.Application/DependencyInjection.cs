using ST10538419_CLDV7112_Practicium.Application.Records;
using Microsoft.Extensions.DependencyInjection;

namespace ST10538419_CLDV7112_Practicium.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IReferenceRecordService, ReferenceRecordService>();
        return services;
    }
}
