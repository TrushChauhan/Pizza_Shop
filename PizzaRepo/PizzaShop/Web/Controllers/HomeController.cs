using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Service.Interfaces;
using Entity.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public HomeController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        public IActionResult Index()
        {
            if (Request.Cookies["Email"] != null)
            {
                return RedirectToAction("Users", "Main");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserDemo model)
        {
            var success = await _authService.LoginAsync(model.Email, model.Password);
            if (!success)
            {
                TempData["Message"] = "Invalid credentials";
                return RedirectToAction("Index");
            }

            var userRole = await _authService.GetRoleByEmailAsync(model.Email);
            string userName = await _authService.GetUserNameByEmailAsync(model.Email);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, userRole)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            HttpContext.Session.SetString("Token", tokenString);
            HttpContext.Session.SetString("Email", model.Email);
            HttpContext.Session.SetString("Role", userRole);

            var cookieOptions = new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("Jwt", tokenString, cookieOptions);
            
            if (model.IsRemember)
            {
                Response.Cookies.Append("Email", model.Email, cookieOptions);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Role, userRole)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.IsRemember,
                    ExpiresUtc = model.IsRemember ? DateTime.UtcNow.AddDays(30) : null
                });

            return RedirectToAction("Users", "Main");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> ForgotPassword(string email)
        {
            var userExists = await _authService.IsUserExistsAsync(email);
            if (!userExists)
            {
                TempData["Message"] = "Enter right Email Address";
                return RedirectToAction("Index", "Home");
            }
            
            var vm = new userEmail(email);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailLink(userEmail model)
        {
            string toEmail = model.Email;
            await SendEmailAsync(toEmail);
            return RedirectToAction("Index", "Home");
        }

        private async Task SendEmailAsync(string toEmail)
        {
            await _authService.SendPasswordResetEmailAsync(toEmail);
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            return View(new ResetPasswordModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userExists = await _authService.IsUserExistsAsync(model.Email);
            if (!userExists)
            {
                return RedirectToAction("Index", "Home");
            }

            await _authService.ResetPasswordAsync(model.Email, model.NewPassword);

            return RedirectToAction("Index", "Home");
        }
    }
}