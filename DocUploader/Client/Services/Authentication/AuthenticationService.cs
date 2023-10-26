using Blazored.LocalStorage;
using DocUploader.Client.Providers;
using DocUploader.Client.Services.Base;
using DocUploader.Shared.Dtos.User;
using Microsoft.AspNetCore.Components.Authorization;

namespace DocUploader.Client.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService 
    {
        private readonly IClient httpClient;
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public AuthenticationService(IClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
            this.authenticationStateProvider = authenticationStateProvider;
        }
        public async Task<bool> AuthenticateAsync(LoginUserDto loginModel)
        {
            var response = await httpClient.LoginAsync(loginModel);

            // Store Token
            await localStorage.SetItemAsync("accessToken", response.Token);

            // Change auth state of app
            await ((ApiAuthenticationStateProvider)authenticationStateProvider).LoggedIn();


            return true;

        }



        public async Task Logout()
        {
            // Change auth state of app
            await ((ApiAuthenticationStateProvider)authenticationStateProvider).LoggedOut();
        }
    }
}
