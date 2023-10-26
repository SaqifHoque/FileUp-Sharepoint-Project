

using Microsoft.AspNetCore.Identity;

namespace DocUploader.Shared.AuthModels
{
    public class ApiUser : IdentityUser
    {
        public string? ClientName { get; set; }
        public string? Last4Digits { get; set; }
    }
}
