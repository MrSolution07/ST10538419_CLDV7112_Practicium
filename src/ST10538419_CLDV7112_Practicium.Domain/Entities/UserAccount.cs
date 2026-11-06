using ST10538419_CLDV7112_Practicium.Domain;

namespace ST10538419_CLDV7112_Practicium.Domain.Entities;

// Task (b): one Azure SQL login. The password is stored only as a hash. Role is Farmer, Agronomist, or Admin.
public sealed class UserAccount : Entity
{
    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = UserRoles.Farmer;
}
