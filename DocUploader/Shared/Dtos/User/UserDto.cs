using System.ComponentModel.DataAnnotations;

namespace DocUploader.Shared.Dtos.User
{
    public class UserDto : LoginUserDto
    {
        [Required]
        public string? ClientName { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage ="Must be 4 digits")]
        public string? Last4Digits { get; set; }

        [Required]
        public string? Role { get; set; }

        [Required]
        [Compare("Password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{12,}$", ErrorMessage = "Password must be 12 characters long with Uppercase, Lowercase, Number, Special Character")]
        public string? ConfirmPassword { get; set; }
    }
}
