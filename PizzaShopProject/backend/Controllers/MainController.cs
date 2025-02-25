
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System.IO.Pipelines;
using System.Net;
using static Microsoft.AspNetCore.Http.HttpContext;
namespace backend.Controllers;

public class MainController : Controller
{
    private readonly ILogger<MainController> _logger;

    private readonly PostgresContext _dbcontext;
    public MainController(ILogger<MainController> logger, PostgresContext context)
    {
        _logger = logger;
        this._dbcontext = context;
    }
    public IActionResult Logout()
    {   HttpContext.Session.Clear();
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }
        return RedirectToAction("Index","Home");
    }
    public IActionResult Content()
    {
        return View();
    }
}