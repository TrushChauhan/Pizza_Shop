using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Entity.Models;
using System.IO.Pipelines;
using System.Net;
using static Microsoft.AspNetCore.Http.HttpContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using Entity.ViewModel;
using AspNetCoreGeneratedDocument;
using Service.Interfaces;
namespace Web.Controllers;
public class MainController : Controller
{

    private readonly IConfiguration _config;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IRolesAndPermissionsService _rolesPermissionsService;
    public MainController(IUserService userService, IConfiguration config, IAuthService authService, IEmailService emailService,IRolesAndPermissionsService rolesAndPermissionsService)
    {
        _config = config;
        _userService = userService;
        _authService = authService;
        _emailService = emailService;
        _rolesPermissionsService= rolesAndPermissionsService;
    }
    [Authorize]
    public IActionResult Users()
    {
        ShowUsers();
        return View();
    }
    public IActionResult ShowUsers(string searchTerm = null, int page = 1, int pageSize = 2)
    {
        var (users, totalItems) = _userService.GetUsers(searchTerm, page, pageSize);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalItems = totalItems;
        ViewBag.SearchTerm = searchTerm;
        return View("Users", users);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }
        return RedirectToAction("Index", "Home");
    }
    public IActionResult ChangePassword()
    {
        return View();
    }
    [HttpPost]
    public IActionResult DoChangePassword(ChangePasswordModel model)
    {

        string userEmail = User.Identity.Name;

        if (userEmail == null)
        {
            Logout();
        }
        bool isCorrectPassword = _authService.IsCorrectPassword(userEmail, model.CurrentPassword);

        if (isCorrectPassword)
        {
            _authService.ResetPassword(userEmail, model.ConfirmNewPassword);
        }
        else
        {
            TempData["Message"] = "Invalid Password";
            return RedirectToAction("ChangePassword", "Main");
        }
        return RedirectToAction("Index", "Home");
    }
    public IActionResult AddNewUser()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddNewUser(AddUserDetail model)
    {
        _emailService.SendEmailToNewUser(model.Email, model.Password);
        _userService.AddNewUser(model);
        return RedirectToAction("Users");
    }


    [HttpPost]
    public IActionResult Delete(int id)
    {
        bool Isdeleted = _userService.DeleteUser(id);

        if (Isdeleted)
        {
            return Ok(new { success = true, message = "User deleted successfully" });
        }

        return NotFound(new { success = false, message = "User not found" });
    }



    [HttpGet]
    public async Task<ActionResult> LoadCountry()
    {
        var countries = await _userService.GetCountriesAsync();
        var result = countries.Select(c => new { countryid = c.Countryid, name = c.Name });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetState(int countryId)
    {
        var states = await _userService.GetStatesByCountryAsync(countryId);
        var result = states.Select(s => new { stateid = s.Stateid, name = s.Name });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetCity(int stateId)
    {
        var cities = await _userService.GetCitiesByStateAsync(stateId);
        var result = cities.Select(c => new { cityid = c.Cityid, name = c.Name });
        return Ok(result);
    }

    [Authorize(policy: "AdminOnly")]
    public IActionResult Dashboard()
    {
        return View();
    }
    [Authorize(policy: "AdminOnly")]
    public IActionResult Roles()
    {
        var roles = _userService.GetRoles();
        return View(roles);
    }

    [Authorize(policy: "AdminOnly")]
    public IActionResult Permissions(int roleId)
    {
        var permissions=_rolesPermissionsService.GetPermissionsByRole(roleId);
        var role= _authService.GetRoleById(roleId);

        ViewBag.RoleName = role;
        ViewBag.RoleId = roleId;

        return View(permissions);
    }

    [HttpPost]
    [Authorize(policy: "AdminOnly")]
    public IActionResult UpdatePermissions(int roleId, List<PermissionUpdateModel> permissions)
    {
        _rolesPermissionsService.UpdatePermissions(roleId,permissions);    
        return Ok(new { success = true });
        
    }
}
