namespace Service.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password, bool rememberMe);
    void SendPasswordResetEmail(string email);
    void ResetPassword(string email, string newPassword);
    string EncryptPassword(string password);
    bool IsUserExists(string email);
    bool IsCorrectPassword(string email,string Password);
    string GetRoleByEmail(string email);
}
