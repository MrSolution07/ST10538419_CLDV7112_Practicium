using System.ComponentModel.DataAnnotations;
using ST10538419_CLDV7112_Practicium.Application.Accounts;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Web.Models;

public sealed class UserAccountForm
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(UserAccountService.UsernameMaxLength, MinimumLength = 3)]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [StringLength(UserAccountService.PasswordMaxLength)]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;

    public UserAccountInput ToInput() => new(Username, Password, Role);

    public static UserAccountForm FromAccount(UserAccount account) => new()
    {
        Id = account.Id,
        Username = account.Username,
        Role = account.Role
    };
}
