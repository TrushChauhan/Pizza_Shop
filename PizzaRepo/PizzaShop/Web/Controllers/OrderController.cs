using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers{
public class OrderController : Controller
{
    public IActionResult Index(){
        return View("Orders");
    }
}
}
