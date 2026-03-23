using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

namespace imsystem
{
    public class EmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _apiKey = configuration["SendGrid:ApiKey"] ?? "";
            _fromEmail = configuration["SendGrid:FromEmail"] ?? "";
            _fromName = configuration["SendGrid:FromName"] ?? "";
        }

        public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail, toName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlBody);
            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }

        public async Task<int> SendBulkEmailAsync(List<(string Email, string Name)> recipients, string subject, string htmlBody)
        {
            int successCount = 0;
            foreach (var recipient in recipients)
            {
                var success = await SendEmailAsync(recipient.Email, recipient.Name, subject, htmlBody);
                if (success) successCount++;
            }
            return successCount;
        }
    }
}