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
    public MainController(IUserService userService,IConfiguration config,IAuthService authService,IEmailService emailService)
    {
        _config=config;
        _userService=userService;
        _authService=authService;
        _emailService=emailService;
    }
    [Authorize]
    public IActionResult Users()
    {
        ShowUsers();
        return View();
    }
    public IActionResult ShowUsers(string searchTerm = null, int page = 1, int pageSize = 2)
    {
        var (users,totalItems)=_userService.GetUsers(searchTerm,page,pageSize);
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
        bool isCorrectPassword = _authService.IsCorrectPassword(userEmail,model.CurrentPassword);

        if (isCorrectPassword)
        {    
            _authService.ResetPassword(userEmail,model.ConfirmNewPassword);
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
        if(!ModelState.IsValid){
            return View(model);
        }
        _userService.AddNewUser(model);
        _emailService.SendEmailToNewUser(model.Email,model.Password);
        return RedirectToAction("Users");
    }


//     [HttpPost]
//     public IActionResult Delete(int id)
//     {
//         try
//         {
//             using (var transaction = _dbcontext.Database.BeginTransaction())
//             {
//                 var userDetail = _dbcontext.Userdetails.FirstOrDefault(u => u.Userid == id);
//                 var userLogin = _dbcontext.Userlogins.FirstOrDefault(u => u.Userid == id);

//                 if (userDetail != null && userLogin != null)
//                 {
//                     userDetail.Isdeleted = true;
//                     userLogin.Isdeleted = true;

//                     _dbcontext.SaveChanges();
//                     transaction.Commit();
//                     return Ok(new { success = true, message = "User deleted successfully" });
//                 }

//                 return NotFound(new { success = false, message = "User not found" });
//             }
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error deleting user");
//             return StatusCode(500, new { success = false, message = "Error deleting user" });
//         }
//     }

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


    [HttpGet]
    public async Task<ActionResult> LoadRoles()
    {
        var roles = await _userService.GetRolesAsync();
        var result = roles.Select(c => new { roleid = c.Roleid, name = c.Rolename });
        return Ok(roles);
    }
    
//     [Authorize(policy: "AdminOnly")]
//     public IActionResult Dashboard()
//     {
//         return View();
//     }
//     [Authorize(policy: "AdminOnly")]
// public IActionResult Roles()
// {
//     var roles = _dbcontext.Userroles
//         .Where(r => !r.Isdeleted)
//         .ToList();
//     return View(roles);
// }

// [Authorize(policy: "AdminOnly")]
// public IActionResult Permissions(int roleId)
// {
//     var role = _dbcontext.Userroles
//         .Include(r => r.Roleandpermissions)
//         .FirstOrDefault(r => r.Roleid == roleId && !r.Isdeleted);

//     if (role == null)
//     {
//         return NotFound();
//     }

//     var rolePermissions = role.Roleandpermissions
//         .ToDictionary(rp => rp.Permissionid);

//     var permissions = _dbcontext.Userpermissions
//         .Where(p => !p.Isdeleted)
//         .AsEnumerable() 
//         .Select(p => 
//         {
//             rolePermissions.TryGetValue(p.Permissionid, out var rp);
//             return new PermissionViewModel
//             {
//                 PermissionId = p.Permissionid,
//                 PermissionName = p.Permissionname,
//                 CanView = rp?.Canview ?? false,
//                 CanAddEdit = rp?.Canaddedit ?? false,
//                 CanDelete = rp?.Candelete ?? false
//             };
//         })
//         .ToList();

//     ViewBag.RoleName = role.Rolename;
//     ViewBag.RoleId = role.Roleid;
    
//     return View(permissions);
// }

// [HttpPost]
// [Authorize(policy: "AdminOnly")]
// public IActionResult UpdatePermissions(int roleId, [FromBody] List<PermissionUpdateModel> permissions)
// {
//     try
//     {
//         var existingPermissions = _dbcontext.Roleandpermissions
//             .Where(rp => rp.Roleid == roleId)
//             .ToList();

//         _dbcontext.Roleandpermissions.RemoveRange(existingPermissions);

//         foreach (var perm in permissions)
//         {
//             _dbcontext.Roleandpermissions.Add(new Roleandpermission
//             {
//                 Roleid = roleId,
//                 Permissionid = perm.PermissionId,
//                 Canview = perm.CanView,
//                 Canaddedit = perm.CanAddEdit,
//                 Candelete = perm.CanDelete,
//                 Createddate = DateTime.Now,
//             });
//         }
//         _dbcontext.SaveChanges();
//         return Ok(new { success = true });
//     }
//     catch (Exception ex)
//     {
//         _logger.LogError(ex, "Error updating permissions");
//         return StatusCode(500, new { success = false });
//     }
// }
}
