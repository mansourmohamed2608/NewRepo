namespace GAMAX.Services.Services
{
    public interface IMailingService
    {
        Task SendEmailWithAttachmentAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null);
        Task SendEmailAsync(string mailTo, string subject, string body);
    }
}
