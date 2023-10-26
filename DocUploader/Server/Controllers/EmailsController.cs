using DocUploader.Server.Data;
using DocUploader.Server.EmailServiceManager.Services;
using DocUploader.Shared.Dtos.User;
using DocUploader.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace DocUploader.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public EmailsController(IEmailService emailService, ApplicationDbContext context)
        {
            _emailService = emailService;
            _context = context;
        }

        [HttpPost("send")]
        public async Task<IActionResult> EmailSender(UserDto userDto)
        {
            var token = Guid.NewGuid().ToString();
            var emailTemplate = $@"Dear {userDto.ClientName},

Thank you for choosing to register with External Portal! Before you can start upload documents in of our website,
we kindly request you to active your account. To activate your account, please click on the following link: https://localhost:7202/verified/{token}
By clicking on the activation link, you will be directed to a secure page. Where you can complete the activation process.
If the link is not clickable you can copy and paste it into your web broser's address bar.

Best regards,
External Portal Admin";
            var message = new EmailServiceManager.Message(new string[] { userDto.Email! },
                    "Activate Your Account - Welcome to External Portal", emailTemplate);

            TwoFactorAuth twoFactorAuth = new TwoFactorAuth()
            {
                Email = userDto.Email,
                Token = token,
            };

            await _context.TwoFactorAuths.AddAsync(twoFactorAuth);
            await _context.SaveChangesAsync();

            _emailService.SendEmail(message);
            return Ok();
        }

        [HttpPost("verify")]
        public async Task<IActionResult> EmailVerify(TwoFactorAuth twoFactorAuth)
        {
            await _context.TwoFactorAuths.AddAsync(twoFactorAuth);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("verificationChecker/{token}")]
        public async Task<ActionResult<TwoFactorAuth>> IsVerified(string token)
        {
            var record = await _context.TwoFactorAuths.Where(x=> x.Token==token).FirstOrDefaultAsync();

            var newRecord = record;
            if (record != null)
            {
                return Ok(record);
            }
            else
            {
                return NotFound();
            }

        }
    }
}
