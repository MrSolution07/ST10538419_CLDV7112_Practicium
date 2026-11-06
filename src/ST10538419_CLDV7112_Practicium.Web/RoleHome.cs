using System.Security.Claims;
using ST10538419_CLDV7112_Practicium.Domain;

namespace ST10538419_CLDV7112_Practicium.Web;

public static class RoleHome
{
    public static string ControllerFor(string? role) => role switch
    {
        UserRoles.Farmer => "Dashboard",
        UserRoles.Agronomist => "Monitor",
        UserRoles.Admin => "Operations",
        _ => "Account"
    };

    public static string ControllerFor(ClaimsPrincipal user) =>
        ControllerFor(user.FindFirstValue(ClaimTypes.Role));
}
