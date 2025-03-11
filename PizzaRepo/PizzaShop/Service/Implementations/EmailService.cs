
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;

namespace Service.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendPasswordResetEmail(string email)
    {
        var smtpSettings = _config.GetSection("SMTP");
        using var client = new SmtpClient(smtpSettings["Host"])
        {
            Port = int.Parse(smtpSettings["Port"]),
            Credentials = new NetworkCredential(
                smtpSettings["Username"], 
                smtpSettings["Password"]
            ),
            EnableSsl = true
        };

        var mail = new MailMessage(
            from: smtpSettings["From"],
            to: email)
        {
            Subject = "Password Reset",
            Body = GenerateResetEmailBody(email),
            IsBodyHtml = true
        };

        client.Send(mail);
    }

    private string GenerateResetEmailBody(string email)
    {
        return $@"<a href='{_config["BaseUrl"]}/Home/ResetPassword?email={
            WebUtility.UrlEncode(email)}'>Reset Password</a>";
    }
}