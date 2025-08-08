using EmpManage.Interfaces;
using System.Net;
using System.Net.Mail;

namespace EmpManage.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private IConfiguration _config;

        public EmailRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSection = _config.GetSection("SMTP");

            string senderEmail = smtpSection["UserName"]?.Trim();
            string recipientEmail = toEmail?.Trim();

            // Validate sender
            if (string.IsNullOrWhiteSpace(senderEmail))
                throw new ArgumentException("Sender email is missing or invalid.");

            // Validate recipient
            if (string.IsNullOrWhiteSpace(recipientEmail))
                throw new ArgumentException("Recipient email is missing or invalid.");

            try
            {
                var smtpClient = new SmtpClient(smtpSection["Host"])
                {
                    Port = int.Parse(smtpSection["Port"]),
                    Credentials = new NetworkCredential(senderEmail, smtpSection["Password"]),
                    EnableSsl = bool.Parse(smtpSection["EnableSsl"])
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                // Construct MailAddress explicitly to catch format issues
                var recipientAddress = new MailAddress(recipientEmail);
                mailMessage.To.Add(recipientAddress);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"❌ Invalid email format: {toEmail}");
                throw new FormatException($"Email address is not in a valid format: {toEmail}", ex);
            }
        }



    }
}
