namespace Service.Interfaces;

public interface IEmailService
{
    void SendPasswordResetEmail(string email);
}
