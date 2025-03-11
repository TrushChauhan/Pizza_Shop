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
namespace Web.Controllers;

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
            return RedirectToAction("Content", "Main");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserDemo model)
    {
        var success = await _authService.LoginAsync(model.Email, model.Password, model.IsRemember);
        if (success)
        {   
            var userRole = _authService.GetRoleByEmail(model.Email);

            ViewBag.User = model;
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

            if (model.IsRemember)
            {
                CookieOptions cookie = new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(30)
                };
                Response.Cookies.Append("Token", tokenString, cookie);
                Response.Cookies.Append("Email", model.Email, cookie);
                Response.Cookies.Append("Jwt", tokenString, cookie);
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Email),
            new Claim(ClaimTypes.Role, userRole)
        };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                "Cookies",
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.IsRemember,
                    ExpiresUtc = model.IsRemember ? DateTime.UtcNow.AddDays(30) : null
                });
            return RedirectToAction("Content", "Main");
        }
        
        TempData["Message"] = "Invalid credentials";
        return RedirectToAction("Index");
    }

    private string GenerateJwtToken(string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
