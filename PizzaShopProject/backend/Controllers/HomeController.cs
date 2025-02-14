using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System.IO.Pipelines;

namespace backend.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly PostgresContext _dbcontext;
    public HomeController(ILogger<HomeController> logger,PostgresContext context)
    {
        _logger = logger;
        this._dbcontext=context;
    }

    public IActionResult Index()
    {
        string UserEmail = Request.Cookies["Email"];
        if(UserEmail != null) {
            HttpContext.Session.SetString("Email",UserEmail.ToString());
            return RedirectToAction("Content", "Main");
        }
        return View();
    }
   
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [Route("", Name = "")]
        public IActionResult Login(UserDemo model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(model.IsRemember){
                    CookieOptions cookie = new CookieOptions(){
                        Expires = DateTime.Now.AddDays(30)
                    };

                    Response.Cookies.Append("Email",model.Email,cookie);
                    Response.Cookies.Append("Password",model.Password,cookie);
                }
            var existingUser = _dbcontext.Userlogins.FirstOrDefault(u => u.Email == model.Email || u.Password == model.Password);
            if (existingUser != null)
            {
                return RedirectToPage("Index");

            }
            return RedirectToPage("Index");
        }        
    public IActionResult ResetPassword()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
