using DocUploader.Shared.Dtos.User;
using DocUploader.Shared.Models;

namespace DocUploader.Client.Services.EmailManager
{
    public interface IEmailManager
    {
        Task<bool> SendEmail(UserDto userDto);
        Task<bool> CheckVerification(string token);
    }
}
