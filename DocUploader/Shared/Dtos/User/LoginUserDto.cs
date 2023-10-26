using System.ComponentModel.DataAnnotations;

namespace DocUploader.Shared.Dtos.User
{
    public class LoginUserDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{12,}$", ErrorMessage = "Password must be 12 characters long with Uppercase, Lowercase, Number, Special Character")]
        public string? Password { get; set; }
    }
}
