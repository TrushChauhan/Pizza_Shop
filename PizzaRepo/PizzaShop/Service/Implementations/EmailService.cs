
using MimeKit;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendPasswordResetEmail(string email)
        {
            try
        {
            string emailBody = $@"<a href='http://localhost:5150/Home/ResetPassword?email={WebUtility.UrlEncode(email)}'>Reset Password</a>";
            string smtpEmail = "test.dotnet@etatvasoft.com";
            string smtpappPass = "P}N^{z-]7Ilp";

            SmtpClient smtpclient = new SmtpClient("mail.etatvasoft.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(smtpEmail, smtpappPass),
                EnableSsl = true,
            };

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(smtpEmail),
                Subject = "Reset Password for PizzaShop",
                Body = emailBody,
                IsBodyHtml = true,

            };
            mail.IsBodyHtml = true;
            mail.To.Add(email);
            smtpclient.Send(mail);
            return;
        }
        catch (Exception ex)
        {
            throw (ex);
        }
        }
    }
}