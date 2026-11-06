using ST10538419_CLDV7112_Practicium.Domain;

namespace ST10538419_CLDV7112_Practicium.Application.Accounts;

public sealed record DemoAccount(string Username, string Password, string Role);

public static class DemoAccounts
{
    public static readonly DemoAccount[] All =
    [
        new(Username: "farmer", Password: "Farmer-field-1", Role: UserRoles.Farmer),
        new(Username: "agronomist", Password: "Agronomist-field-1", Role: UserRoles.Agronomist),
        new(Username: "admin", Password: "Admin-field-1", Role: UserRoles.Admin)
    ];
}
