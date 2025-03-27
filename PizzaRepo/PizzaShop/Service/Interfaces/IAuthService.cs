namespace Service.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> IsCorrectPasswordAsync(string email, string password);
    Task SendPasswordResetEmailAsync(string email);
    Task<bool> IsUserExistsAsync(string email);
    Task ResetPasswordAsync(string email, string newPassword);
    Task<string> GetUserNameByEmailAsync(string email);
    Task<string> GetRoleByEmailAsync(string email);
    Task<string> GetRoleByIdAsync(int roleId);
    string EncryptPassword(string pass);
    Task<int> GetUserIdByEmailAsync(string email);
}