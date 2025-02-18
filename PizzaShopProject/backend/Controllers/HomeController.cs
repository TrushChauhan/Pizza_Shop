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
        string UserEmail = HttpContext.Session.GetString("Email");
        if(UserEmail != null) {
            var existingUser = _dbcontext.Userlogins.Any(u => u.Email == UserEmail);
            if(existingUser){
            return RedirectToAction("Content", "Main");
            }
        }
        return View();
    }
    public IActionResult ForgotPassword()
    {
        string userEmail=HttpContext.Session.GetString("Email");
        var model=new UserDemo{Email=userEmail};
        return View(model);
    }

    [HttpPost]
        public IActionResult Login(UserDemo model)
        { 
            var existingUser = _dbcontext.Userlogins.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
            if (existingUser != null)
            {
                HttpContext.Session.SetString("Email",model.Email);
                if(model.IsRemember){
                    CookieOptions cookie = new CookieOptions(){
                        Expires = DateTime.Now.AddDays(30)
                    };

                    Response.Cookies.Append("Email",model.Email,cookie);
                }
                else return RedirectToAction("Content","Main");
            }
            return RedirectToAction("Index","Home");
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
