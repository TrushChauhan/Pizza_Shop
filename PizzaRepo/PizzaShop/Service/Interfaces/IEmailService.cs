namespace Service.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email);
    Task SendEmailToNewUserAsync(string email,string Password); 
}
