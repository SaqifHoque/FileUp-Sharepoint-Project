global using DocUploader.Shared.Models;
using DocUploader.Server.Data;
using DocUploader.Server.EmailServiceManager.Services;
using DocUploader.Server.EmailServiceManager;
using DocUploader.Shared.AuthModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using DocUploader.Client.Services.SharePoint;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
//MySQL DB Conn
string _GetConnStringName = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseMySql(_GetConnStringName, ServerVersion.AutoDetect(_GetConnStringName)));


//Email Config Start
var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();

builder.Services.AddSingleton(emailConfig!);
builder.Services.AddScoped<IEmailService, EmailService>();
//Email Config End

//SharePoint Config Start
var sharePointConfig = builder.Configuration
    .GetSection("SharePointConfiguration")
    .Get<SharePointConfiguration>();

builder.Services.AddSingleton(sharePointConfig!);
builder.Services.AddScoped<ISharePointService, SharePointService>();
//SharePoint Config End

builder.Services.AddScoped<IEmailService, EmailService>();

//Identity Start
builder.Services.AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
//Identity End

//Serilog Start
builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));
//Serilog End

//CORS Start
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll",
        b => b.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());
});
//CORS End

//JWT Start
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
    };
});
//JWT End

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
    RequestPath = "/Files"
});
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
