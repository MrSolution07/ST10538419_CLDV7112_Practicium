using ST10538419_CLDV7112_Practicium.Domain;

namespace ST10538419_CLDV7112_Practicium.Domain.Entities;

public sealed class Device : Entity
{
    public string Name { get; set; } = string.Empty;

    public string FieldName { get; set; } = string.Empty;

    public string OperationalStatus { get; set; } = DeviceStatuses.Idle;
}
