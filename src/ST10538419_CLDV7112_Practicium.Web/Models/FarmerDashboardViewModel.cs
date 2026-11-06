using ST10538419_CLDV7112_Practicium.Application.Farm;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Web.Models;

public sealed class FarmerDashboardViewModel
{
    public required HostingStatusViewModel Hosting { get; init; }

    public required IReadOnlyList<Device> Devices { get; init; }

    public required IReadOnlyList<SensorReading> Readings { get; init; }

    public required FarmGalleryModel Gallery { get; init; }

    public string? TelemetryMessage { get; init; }
}
