using AutoMapper;
using DocUploader.Server.Data;
using DocUploader.Server.Static;
using DocUploader.Shared.AuthModels;
using DocUploader.Shared.Dtos.User;
using DocUploader.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DocUploader.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private readonly UserManager<ApiUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(ILogger<AuthController> logger, UserManager<ApiUser> userManager,
            IConfiguration configuration, ApplicationDbContext context)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            logger.LogInformation($"(Registration Attempt for {userDto.Email} )");

           
            try
            {
                var user = new ApiUser()
                {
                    Email = userDto.Email,
                    ClientName = userDto.ClientName,
                    Last4Digits = userDto.Last4Digits,
                   
                };
                
                user.UserName = userDto.Email;
                var result = await userManager.CreateAsync(user, userDto.Password!);

                

                await userManager.AddToRoleAsync(user, "User");
                // I am just Adding User Role. But I can also accept Admin
                //Example:

                //await userManager.AddToRoleAsync(user, userDto.Role);

                //If anyone pass "role":"Administrator", he will be an Administrator


                await _context.SaveChangesAsync();

                var client = new Shared.Models.Client()
                {
                    ClientName = userDto.ClientName,
                    Email = userDto.Email,
                    Last4Digits = userDto.Last4Digits,
                    Password = userDto.Password
                };


                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();

                var request = new Request()
                {
                    RequestName = userDto.ClientName
                };

                await _context.Requests.AddAsync(request);
                await _context.SaveChangesAsync();

                return Accepted();
            }
            catch (Exception ex)
            {
               
                await _context.SaveChangesAsync();

                logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)}");
                return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginUserDto userDto)
        {
            logger.LogInformation($"Login Attempt for {userDto.Email} ");
          
            try
            {
                var user = await userManager.FindByEmailAsync(userDto.Email!);
                var passwordValid = await userManager.CheckPasswordAsync(user!, userDto.Password!);

                if (user == null || passwordValid == false)
                {
                    return Unauthorized(userDto);
                }

                string tokenString = await GenerateToken(user);

                var response = new AuthResponse
                {
                    Email = userDto.Email,
                    Token = tokenString,
                    UserId = user.Id,
                };

               
                await _context.SaveChangesAsync();

                return response;
            }
            catch (Exception ex)
            {
                
                await _context.SaveChangesAsync();

                logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)}");
                return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: 500);
            }
        }
        //Email, UserName, Password, FirstName, LastName all are Claim 
        private async Task<string> GenerateToken(ApiUser user)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

            var userClaims = await userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(CustomClaimTypes.Uid, user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToInt32(configuration["JwtSettings:Duration"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
