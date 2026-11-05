namespace ST10538419_CLDV7112_Practicium.Domain.Entities;

/// <summary>
/// Sample aggregate. Copy this shape for each entity the brief requires, then remove it if the brief does not need it.
/// </summary>
public sealed class ReferenceRecord : Entity
{
    public string Title { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public bool IsArchived { get; set; }
}
