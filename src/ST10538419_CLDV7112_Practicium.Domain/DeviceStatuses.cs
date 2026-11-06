namespace ST10538419_CLDV7112_Practicium.Domain;

public static class DeviceStatuses
{
    public const string Idle = "Idle";

    public const string Irrigating = "Irrigating";

    public const string NeedsAttention = "NeedsAttention";

    public static readonly string[] All = [Idle, Irrigating, NeedsAttention];

    public static bool IsKnown(string? status) =>
        status is not null && All.Contains(status.Trim());
}
