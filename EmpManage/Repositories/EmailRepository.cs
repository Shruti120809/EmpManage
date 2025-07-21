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
            var smtpsection = _config.GetSection("SMTP");

            var smtpClient = new SmtpClient(smtpsection["Host"])
            {
                Port = int.Parse(smtpsection["Port"]!),
                Credentials = new NetworkCredential(smtpsection["UserName"], smtpsection["Password"]),
                EnableSsl = bool.Parse(smtpsection["EnableSsl"]!)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpsection["UserName"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
