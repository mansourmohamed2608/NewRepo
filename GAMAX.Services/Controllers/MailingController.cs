using GAMAX.Services.Dto;
using GAMAX.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GAMAX.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailingController : ControllerBase
    {
        private readonly IMailingService _mailingService;

        public MailingController(IMailingService mailingService)
        {
            _mailingService = mailingService;
        }

        [HttpPost("sendWithAttachment")]
        public async Task<IActionResult> SendMailWithAttachment([FromForm] MailRequestWithAttachmentDto dto)
        {
            await _mailingService.SendEmailWithAttachmentAsync(dto.ToEmail, dto.Subject, dto.Body, dto.Attachments);
            return Ok();
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] MailRequestDto dto)
        {
            await _mailingService.SendEmailAsync(dto.ToEmail, dto.Subject, dto.Body);
            return Ok();
        }

        [HttpPost("welcome")]
        public async Task<IActionResult> SendWelcomeEmail([FromBody] WelcomeRequestDto dto)
        {
            var filePath = $"{Directory.GetCurrentDirectory()}\\Templates\\EmailTemplate.html";
            var str = new StreamReader(filePath);

            var mailText = str.ReadToEnd();
            str.Close();

            mailText = mailText.Replace("[username]", dto.UserName).Replace("[email]", dto.Email);

            await _mailingService.SendEmailAsync(dto.Email, "Welcome to our channel", mailText);
            return Ok();
        }
    }
}
