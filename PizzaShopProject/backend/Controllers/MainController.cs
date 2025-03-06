using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System.IO.Pipelines;
using System.Net;
using static Microsoft.AspNetCore.Http.HttpContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
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
    [Authorize]
     public IActionResult Content()
    {
        Users();
        return View();
    }
public IActionResult Users(string searchTerm = null, int page = 1, int pageSize = 10)
{
    var query = _dbcontext.Userdetails
        .Include(ud => ud.User)       
        .Include(ud => ud.Role)       
        .Where(ud => !ud.Isdeleted && !ud.User.Isdeleted);

    if (!string.IsNullOrEmpty(searchTerm))
    {
        searchTerm = searchTerm.ToLower();
        query = query.Where(ud => 
            (ud.Firstname + " " + ud.Lastname).ToLower().Contains(searchTerm) ||
            ud.User.Email.ToLower().Contains(searchTerm) ||
            ud.Phonenumber.Contains(searchTerm) ||
            ud.Role.Rolename.ToLower().Contains(searchTerm)
        );
    }

    var totalItems = query.Count();
    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    
    var users = query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(ud => new UserTable
        {
            UserId = ud.Userid,
            Name = $"{ud.Firstname} {ud.Lastname}",
            Email = ud.User.Email,
            Phone = ud.Phonenumber,
            Role = ud.Role.Rolename,
            Status = ud.Status ? "active" : "inactive",
            ProfileImage = ud.Profileimage 
        })
        .ToList();
    ViewBag.CurrentPage = page;
    ViewBag.PageSize = pageSize;
    ViewBag.TotalPages = totalPages;
    ViewBag.TotalItems = totalItems;
    ViewBag.SearchTerm = searchTerm;

    return View("Content", users);
}

public IActionResult Logout()
    {   
        HttpContext.Session.Clear();
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }
        return RedirectToAction("Index","Home");
    }
public IActionResult ChangePassword(){
    return View();
}
[HttpPost]
public IActionResult DoChangePassword(ChangePassword model){
    
 string userEmailString = User.Identity.Name;
    Console.Write(userEmailString);
    if(userEmailString == null){
        Logout();
    }
    var user = _dbcontext.Userlogins.FirstOrDefault(u => u.Email == userEmailString);
     
     if(encryptPassword(model.CurrentPassword)==user.Password){
        user.Password = encryptPassword(model.NewPassword);
        _dbcontext.SaveChanges();
     }
      else
        {
            TempData["Message"] = "Invalid Password";
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return RedirectToAction("ChangePassword","Main");
        }
    return RedirectToAction("Index","Home");
}
public IActionResult AddNewUser(){
    return View();
}

[HttpPost]
public IActionResult AddNewUser(Userdetail model, IFormFile ProfileImage)
{

        if (ProfileImage != null && ProfileImage.Length > 0)
        {
            var filePath = Path.Combine("wwwroot/images/profiles", ProfileImage.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                ProfileImage.CopyTo(stream);
            }
            model.Profileimage = $"/images/profiles/{ProfileImage.FileName}";
        }

        var nextUserId= _dbcontext.Userdetails.Count()+1;
        model.Userid=nextUserId;
        model.Createddate = DateTime.Now;
        model.Isdeleted = false;
        _dbcontext.Userdetails.Add(model);
        _dbcontext.SaveChanges();

        return RedirectToAction("Content");
    

    return View(model);
}

public string encryptPassword(string pass)
    {
        byte[] encode = new byte[pass.Length];
        encode = System.Text.Encoding.UTF8.GetBytes(pass);
        string encodedData = Convert.ToBase64String(encode);
        return encodedData;
    }


    [HttpPost]
public IActionResult Delete(int id)
{
    try
    {
        using (var transaction = _dbcontext.Database.BeginTransaction())
        {
            var userDetail = _dbcontext.Userdetails.FirstOrDefault(u => u.Userid == id);
            var userLogin = _dbcontext.Userlogins.FirstOrDefault(u => u.Userid == id);

            if (userDetail != null && userLogin != null)
            {
                
                userDetail.Isdeleted = true;
                userLogin.Isdeleted = true;

                _dbcontext.SaveChanges();
                transaction.Commit();
                return Ok(new { success = true, message = "User deleted successfully" });
            }

            return NotFound(new { success = false, message = "User not found" });
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error deleting user");
        return StatusCode(500, new { success = false, message = "Error deleting user" });
    }
}

[HttpGet]
public async Task<ActionResult> LoadCountry()
{
    var countries = await _dbcontext.Countries
        .Where(c => !c.Isdeleted)
        .Select(c => new { countryid = c.Countryid, name = c.Name })
        .ToListAsync();
    return Ok(countries);
}

[HttpGet]
public async Task<ActionResult> GetState(int countryId)
{
    var states = await _dbcontext.States
        .Where(s => s.Countryid == countryId && !s.Isdeleted)
        .Select(s => new { stateid = s.Stateid, name = s.Name })
        .ToListAsync();
    return Ok(states);
}

[HttpGet]
public async Task<ActionResult> GetCity(int stateId)
{
    var cities = await _dbcontext.Cities
        .Where(c => c.Stateid == stateId && !c.Isdeleted)
        .Select(c => new { cityid = c.Cityid, name = c.Name })
        .ToListAsync();
    return Ok(cities);
}
[Authorize (policy:"AdminOnly")]
public IActionResult Permissions(){
    return View();
}
[Authorize (policy:"AdminOnly")]
public IActionResult Dashboard(){
    return View();
}
}