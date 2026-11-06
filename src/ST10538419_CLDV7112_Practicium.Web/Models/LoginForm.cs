using System.ComponentModel.DataAnnotations;

namespace ST10538419_CLDV7112_Practicium.Web.Models;

public sealed class LoginForm
{
    [Required(ErrorMessage = "Username is required.")]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}
