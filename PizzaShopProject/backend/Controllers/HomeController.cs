using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System.IO.Pipelines;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace backend.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly PostgresContext _dbcontext;
    private readonly IConfiguration _config;
    public HomeController(ILogger<HomeController> logger,PostgresContext context,IConfiguration config)
    {
        _logger = logger;
        _dbcontext=context;
        _config=config;
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
        return View();
    }

    [HttpPost]
        public IActionResult Login(UserDemo model)
        {   string userPassword=encryptPassword(model.Password);
            Console.WriteLine(userPassword);
            var existingUser = _dbcontext.Userlogins.FirstOrDefault(u => u.Email == model.Email && u.Password == userPassword);
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

        [HttpPost]
     public IActionResult SendEmailLink(userEmail model){
       string toemail=model.Email;
        SendEmail(toemail);
        return RedirectToAction("Index","Home");
    }
    public void SendEmail(string toemail){
        try{
            string emailBody = @"<html>
                <body>

                    <a href='http://localhost:5150/Home/ResetPassword'> Click Here</a>  
                   </body>
            </html>";
            string smtpEmail = _config.GetValue<string>("SMTPCredentials:Email");
            string smtpappPass = _config.GetValue<string>("SMTPCredentials:AppPass");

            SmtpClient smtpclient = new SmtpClient("mail.etatvasoft.com"){
                Port = 587,
                Credentials = new NetworkCredential(smtpEmail,smtpappPass),
                EnableSsl = true,
            };

            MailMessage mail = new MailMessage{
                From= new MailAddress(smtpEmail),
                Subject = "Reset Password for PizzaShop",
                Body = emailBody,
                IsBodyHtml = true,
                
            };
            mail.IsBodyHtml=true;
            mail.To.Add(toemail);
            smtpclient.Send(mail);
            return ;
        }
        catch(Exception ex){
            throw(ex);  
        }
     }
    public static string encryptPassword(string pass){
        byte[] encode = new byte[pass.Length];
        encode = System.Text.Encoding.UTF8.GetBytes(pass);
        string encodedData = Convert.ToBase64String(encode);
        return encodedData;
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
