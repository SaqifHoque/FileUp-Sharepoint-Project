using Blazored.LocalStorage;
using DocUploader.Client;
using DocUploader.Client.Providers;
using DocUploader.Client.Services.Authentication;
using DocUploader.Client.Services.Base;
using DocUploader.Client.Services.EmailManager;
using DocUploader.Client.Services.FilesManager;
using GoogleCaptchaComponent;
using GoogleCaptchaComponent.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration.Memory;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//Start
builder.Services.AddScoped<IFilesManager, FilesManager>();
builder.Services.AddScoped<IGenericFilesManager, GenericFilesManager>();
builder.Services.AddScoped<IEmailManager, EmailManager>();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(p =>
                p.GetRequiredService<ApiAuthenticationStateProvider>());
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<IClient, Client>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var tokenData = new Dictionary<string, string>()
        {
            //{"CaptchaSiteToken", "6Le50C4mAAAAALeUs0nB5VqZMJo-KPv6Y-sFhB6Z"},
            {"CaptchaSiteToken", "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI"},
            //{"CaptchaSecretToken", "6Le50C4mAAAAABkMi3oaqG_KP0yGfqy7dqqiZUyq"}
            {"CaptchaSecretToken", "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe"}
        };

var memoryConfig = new MemoryConfigurationSource{ InitialData = tokenData! };

builder.Configuration.Add(memoryConfig);

var config = builder.Configuration["CaptchaSiteToken"];

builder.Services.AddGoogleCaptcha(configuration =>
{
    configuration.ServerSideValidationRequired = true;
    configuration.SiteKey = config;
    configuration.CaptchaVersion = CaptchaConfiguration.Version.V2;
});



await builder.Build().RunAsync();
