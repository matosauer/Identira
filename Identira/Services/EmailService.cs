using brevo_csharp.Api;
using brevo_csharp.Model;
using Identira.Models;

namespace Identira.Services
{
    public class EmailService : IEmailService
    {
        private readonly TransactionalEmailsApi _api;
        private readonly EmailSettings _emailSettings;

        public EmailService(IConfiguration config)
        {
            var section = config.GetSection("BrevoEmailSettings");

            _emailSettings = new EmailSettings
            {
                ApiKey = section["ApiKey"] ?? throw new InvalidOperationException("Failed to load API key"),
                SenderEmail = section["SenderEmail"] ?? throw new InvalidOperationException("Failed to load sender email"),
                SenderName = section["SenderName"] ?? throw new InvalidOperationException("Failed to load sender name")
            };

            _api = new TransactionalEmailsApi();
            _api.Configuration.AddApiKey("api-key", _emailSettings.ApiKey);
        }

        public async Task<List<EmailEvent>> GetEmailEventReportAsync(string messageId)
        {
            if (string.IsNullOrWhiteSpace(messageId))
                throw new ArgumentException("Message ID is required", nameof(messageId));

            long? limit = 10;
            long? offset = 0;
            string? startDate = null;
            string? endDate = null;
            long? days = null;
            string? email = null;
            string? _event = null;
            string? tags = null;
            long? templateId = null;
            string? sort = null;

            List<EmailEvent> emailEvents = new List<EmailEvent>();

            // Filter specifically by the messageId you received when sending
            GetEmailEventReport response = await _api.GetEmailEventReportAsync(
                limit, offset, startDate, endDate, days, email, _event, tags,
                messageId,
                templateId, sort);

            // Process the timeline events returned by Brevo
            if (response != null && response.Events != null)
            {
                foreach (var ev in response.Events)
                {
                    emailEvents.Add(new EmailEvent
                    {
                        MessageId = ev.MessageId,
                        Email = ev.Email,
                        Date = DateTime.Parse(ev.Date),
                        Status = ev.Event.ToString(),
                    });
                }

            }
            return emailEvents;
        }

        public async Task<string> SendEmailAsync(string? senderName, string senderEmail, string emailAddress, string subject, string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(senderEmail)) throw new ArgumentException("Sender email is required", nameof(senderEmail));
            if (string.IsNullOrWhiteSpace(emailAddress)) throw new ArgumentException("Email address is required", nameof(emailAddress));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Subject is required", nameof(subject));
            if (string.IsNullOrWhiteSpace(htmlContent)) throw new ArgumentException("HTML content is required", nameof(htmlContent));

            var email = new SendSmtpEmail(

                sender: new SendSmtpEmailSender(senderName, senderEmail),

                to: new List<SendSmtpEmailTo>
                {
                    new SendSmtpEmailTo(emailAddress)
                },

                subject: subject,
                htmlContent: htmlContent
            );

            CreateSmtpEmail result = await _api.SendTransacEmailAsync(email);

            return result.MessageId;
        }

        public async Task<string> SendPasswordResetEmailAsync(string email, string resetLink)
        {
            string subject = "Password Reset";
            string htmlContent = $"<p>Click the following link to reset your password: <a href='{resetLink}'>Reset Password</a></p>";
            return await SendEmailAsync(null, _emailSettings.SenderEmail, email, subject, htmlContent);
        }
    }
}