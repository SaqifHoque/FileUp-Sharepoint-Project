
using DocUploader.Shared.Dtos.User;

namespace DocUploader.Client.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateAsync(LoginUserDto loginModel);
        public Task Logout();
    }
}
