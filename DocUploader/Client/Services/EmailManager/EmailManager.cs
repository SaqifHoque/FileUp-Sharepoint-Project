using DocUploader.Shared.Dtos;
using DocUploader.Shared.Dtos.User;
using DocUploader.Shared.Models;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace DocUploader.Client.Services.EmailManager
{
    public class EmailManager : IEmailManager
    {
        HttpClient _http;
        public EmailManager(HttpClient http)
        {
            _http = http;
        } 

        public async Task<bool> SendEmail(UserDto userDto)
        {
            try
            {
                var result = await _http.PostAsJsonAsync("api/emails/send", userDto);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public async Task<bool> CheckVerification(string token)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<TwoFactorAuth>($"api/emails/verificationChecker/{token}");
                if (result.Token == token)
                {
                    return true;
                }
                else
                {
                    return false;
                }
               
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
