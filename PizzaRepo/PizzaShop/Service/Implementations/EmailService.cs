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
                string emailBody = getEmailBody(email);
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
        private string getEmailBody(string email)
        {
            string resetLink = $"http://localhost:5150/Home/ResetPassword?email={WebUtility.UrlEncode(email)}";

            return $@"
    <html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                padding: 20px;
                text-align: center;
            }}
            .email-container {{
                max-width: 600px;
                margin: 0 auto;
                background: white;
                padding: 20px;
                border-radius: 5px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            }}
            .header {{
                background: #005A9E;
                color: white;
                padding: 15px;
                text-align: center;
                font-size: 24px;
                font-weight: bold;
                border-top-left-radius: 5px;
                border-top-right-radius: 5px;
            }}
            .content {{
                text-align: left;
                padding: 15px;
            }}
            .content a {{
                color: #005A9E;
                text-decoration: none;
                font-weight: bold;
            }}
            .important-note {{
                color: #d9534f;
                font-weight: bold;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <div class='header'>PIZZASHOP</div>
            <div class='content'>
                <p>Pizza shop,</p>
                <p>Please click <a href='{resetLink}'>here</a> to reset your account password.</p>
                <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
                <p class='important-note'>Important Note: For security reasons, the link will expire in 24 hours. If you did not request a password reset, please ignore this email or contact our support team immediately.</p>
            </div>
        </div>
    </body>
    </html>";
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