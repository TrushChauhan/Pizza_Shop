
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System.IO.Pipelines;

namespace backend.Controllers;

public class MainController : Controller
{
    private readonly ILogger<MainController> _logger;

    private readonly PostgresContext _dbcontext;
    public MainController(ILogger<MainController> logger,PostgresContext context)
    {
        _logger = logger;
        this._dbcontext=context;
    }

    public IActionResult Content()
    {
        return View();
    }
}