namespace ST10538419_CLDV7112_Practicium.Domain;

// Task (b): the only three roles stored on a user account.
public static class UserRoles
{
    public const string Farmer = "Farmer";

    public const string Agronomist = "Agronomist";

    public const string Admin = "Admin";

    public static readonly string[] All = [Farmer, Agronomist, Admin];

    public static bool IsKnown(string? role) =>
        role is not null && All.Contains(role.Trim());
}
