using Microsoft.AspNetCore.Mvc;
namespace Web.Controllers;
public class CustomerController : Controller
{
    public async Task<IActionResult> Index(){
        return View("Customers");
    }
    
}
