using ST10538419_CLDV7112_Practicium.Application.Accounts;
using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Application.Farm;
using Microsoft.Extensions.DependencyInjection;

namespace ST10538419_CLDV7112_Practicium.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<ITelemetryQuery, TelemetryQuery>();
        services.AddScoped<IEnvironmentalLogQuery, EnvironmentalLogQuery>();
        services.AddScoped<IImageAuditProcessor, ImageAuditProcessor>();
        services.AddScoped<IIrrigationIntake, IrrigationIntake>();
        services.AddScoped<IIrrigationProcessor, IrrigationProcessor>();
        services.AddScoped<IDroneImageService, DroneImageService>();
        services.AddScoped<IFarmGallery, FarmGallery>();
        return services;
    }
}
