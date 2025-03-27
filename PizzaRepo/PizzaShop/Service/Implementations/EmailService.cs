using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly SmtpClient _smtpClient;

        public EmailService(IConfiguration config)
        {
            _config = config;
            _smtpClient = InitializeSmtpClient();
        }

        private SmtpClient InitializeSmtpClient()
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            return new SmtpClient(smtpSettings["Host"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["Email"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
            };
        }

        private MailMessage CreateMailMessage(string toEmail, string subject, string body)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            return new MailMessage
            {
                From = new MailAddress(smtpSettings["Email"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }.Also(msg => msg.To.Add(toEmail));
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            try
            {
                string emailBody = $@"<a href='http://localhost:5150/Home/ResetPassword?email={WebUtility.UrlEncode(email)}'>Reset Password</a>";
                using var mail = CreateMailMessage(email, "Reset Password for PizzaShop", emailBody);
                await _smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send password reset email.", ex);
            }
        }

        public async Task SendEmailToNewUserAsync(string email, string password)
        {
            try
            {
                string emailBody = $@"  
                <h1>Login Details</h1>  
                <div>Email: {email}</div>  
                <div>Temporary Password: {password}</div>";
                using var mail = CreateMailMessage(email, "Login Details for PizzaShop", emailBody);
                await _smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send new user email.", ex);
            }
        }
    }

    public static class ObjectExtensions
    {
        public static T Also<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }
    }
}