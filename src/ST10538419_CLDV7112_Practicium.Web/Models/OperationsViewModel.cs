namespace ST10538419_CLDV7112_Practicium.Web.Models;

public sealed class OperationsViewModel
{
    public required HostingStatusViewModel Hosting { get; init; }

    public required int AccountCount { get; init; }

    public required int DeviceCount { get; init; }
}
