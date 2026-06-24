namespace Identira.Services
{
    public interface IEmailService
    {
        Task<string> SendEmailAsync(string senderName, string senderEmail, string emailAddress, string subject, string htmlContent);
        Task<string> SendPasswordResetEmailAsync(string email, string resetLink);
    }
}
