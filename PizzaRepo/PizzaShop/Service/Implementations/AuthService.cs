using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using Service.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            IConfiguration config,
            IEmailService emailService)
        {
            _roleRepo = roleRepo;
            _userRepo = userRepo;
            _config = config;
            _emailService = emailService;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var encryptedPass = EncryptPassword(password);
            var user = await _userRepo.GetUserByEmailAsync(email);
            return user?.Password == encryptedPass;
        }

        public async Task<bool> IsCorrectPasswordAsync(string email, string password)
        {
            return await _userRepo.IsCorrectPasswordAsync(email, EncryptPassword(password));
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            if (await _userRepo.IsUserExistsAsync(email))
            {
                await _emailService.SendPasswordResetEmailAsync(email);
            }
        }

        public async Task<bool> IsUserExistsAsync(string email)
        {
            return await _userRepo.IsUserExistsAsync(email);
        }

        public async Task ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            if (user != null)
            {
                user.Password = EncryptPassword(newPassword);
                await _userRepo.UpdateUserLoginAsync(user);
            }
        }

        public async Task<string> GetUserNameByEmailAsync(string email)
        {
            return await _userRepo.GetUserNameByEmailAsync(email);
        }

        public async Task<string> GetRoleByEmailAsync(string email)
        {
            int roleId = await _userRepo.GetRoleIdByEmailAsync(email);
            return await _roleRepo.GetRoleByIdAsync(roleId);
        }

        public async Task<string> GetRoleByIdAsync(int roleId)
        {
            return await _roleRepo.GetRoleByIdAsync(roleId);
        }

        public string EncryptPassword(string pass)
        {
            var encode = Encoding.UTF8.GetBytes(pass);
            return Convert.ToBase64String(encode);
        }

        public async Task<int> GetUserIdByEmailAsync(string email)
        {
            return await _userRepo.GetUserIdByEmailAsync(email);
        }
    }
}