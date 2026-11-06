using ST10538419_CLDV7112_Practicium.Domain;

namespace ST10538419_CLDV7112_Practicium.Domain.Entities;

public sealed class UserAccount : Entity
{
    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = UserRoles.Farmer;
}
