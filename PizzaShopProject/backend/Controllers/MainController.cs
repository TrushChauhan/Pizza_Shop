using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System.IO.Pipelines;
using System.Net;
using static Microsoft.AspNetCore.Http.HttpContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
namespace backend.Controllers;
public class MainController : Controller
{
    private readonly ILogger<MainController> _logger;

    private readonly IConfiguration _config;
    private readonly PostgresContext _dbcontext;
    public MainController(ILogger<MainController> logger, PostgresContext context,IConfiguration config)
    {
        _logger = logger;
        this._dbcontext = context;
        _config=config;
    }
    [Authorize]
    public IActionResult Content()
    {
        Users();
        return View();
    }
    public IActionResult Users(string searchTerm = null, int page = 1, int pageSize = 2)
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
        return RedirectToAction("Index", "Home");
    }
    public IActionResult ChangePassword()
    {
        return View();
    }
    [HttpPost]
    public IActionResult DoChangePassword(ChangePassword model)
    {

        string userEmailString = User.Identity.Name;

        if (userEmailString == null)
        {
            Logout();
        }
        var user = _dbcontext.Userlogins.FirstOrDefault(u => u.Email == userEmailString);

        if (encryptPassword(model.CurrentPassword) == user.Password)
        {
            user.Password = encryptPassword(model.NewPassword);
            _dbcontext.SaveChanges();
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
            return View();
        }
        var nextUserId = _dbcontext.Userdetails.Count() + 1;
        Userdetail userdetail = new Userdetail();
        Userlogin userlogin= new Userlogin();
        DateTime now = DateTime.Now;
        userlogin.Userid=nextUserId;
        userlogin.Email=model.Email;
        userlogin.Password=encryptPassword(model.Password);
        userlogin.Roleid=model.Roleid;
        userdetail.Userid = nextUserId;
        userdetail.Firstname = model.Firstname;
        userdetail.Lastname = model.Lastname;
        userdetail.Username = model.Username;
        userdetail.Address = model.Address;
        userdetail.Cityid = model.Cityid;
        userdetail.Countryid = model.Countryid;
        userdetail.Stateid=model.Stateid;
        userdetail.Roleid=model.Roleid;
        userdetail.Zipcode=model.Zipcode;
        userdetail.Phonenumber=model.Phonenumber;
        userdetail.Status=true;
        userdetail.Createddate=now;
        userdetail.Isdeleted=false;
        _dbcontext.Userlogins.Add(userlogin);
        _dbcontext.Userdetails.Add(userdetail);
        _dbcontext.SaveChanges();
        sendEmailToNewUser(model.Email,model.Password);
        return RedirectToAction("Content");

        // return View(model);
    }
    public void sendEmailToNewUser(string email,string Password){
        try
        {
            string emailBody = $@"
            <h>Login Details</h>
            <div>useremail = {email} </div>
            <div> Temporary Password = {Password}";
            string smtpEmail = _config.GetValue<string>("SMTPCredentials:Email");
            string smtpappPass = _config.GetValue<string>("SMTPCredentials:AppPass");

            SmtpClient smtpclient = new SmtpClient("mail.etatvasoft.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(smtpEmail, smtpappPass),
                EnableSsl = true,
            };

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(smtpEmail),
                Subject = "Login Details for PizzaShop",
                Body = emailBody,
                IsBodyHtml = true,

            };
            mail.IsBodyHtml = true;
            mail.To.Add(email);
            smtpclient.Send(mail);
            return;
        }
        catch (Exception ex)
        {
            throw (ex);
        }
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
    [HttpGet]
    public async Task<ActionResult> LoadRoles()
    {
        var roles = await _dbcontext.Userroles
            .Where(c => !c.Isdeleted)
            .Select(c => new { roleid = c.Roleid, name = c.Rolename })
            .ToListAsync();
        return Ok(roles);
    }
    
    [Authorize(policy: "AdminOnly")]
    public IActionResult Dashboard()
    {
        return View();
    }
    [Authorize(policy: "AdminOnly")]
public IActionResult Roles()
{
    var roles = _dbcontext.Userroles
        .Where(r => !r.Isdeleted)
        .ToList();
    return View(roles);
}

[Authorize(policy: "AdminOnly")]
public IActionResult Permissions(int roleId)
{
    var role = _dbcontext.Userroles
        .Include(r => r.Roleandpermissions)
        .FirstOrDefault(r => r.Roleid == roleId && !r.Isdeleted);

    if (role == null)
    {
        return NotFound();
    }

    var rolePermissions = role.Roleandpermissions
        .ToDictionary(rp => rp.Permissionid);

    var permissions = _dbcontext.Userpermissions
        .Where(p => !p.Isdeleted)
        .AsEnumerable() 
        .Select(p => 
        {
            rolePermissions.TryGetValue(p.Permissionid, out var rp);
            return new PermissionViewModel
            {
                PermissionId = p.Permissionid,
                PermissionName = p.Permissionname,
                CanView = rp?.Canview ?? false,
                CanAddEdit = rp?.Canaddedit ?? false,
                CanDelete = rp?.Candelete ?? false
            };
        })
        .ToList();

    ViewBag.RoleName = role.Rolename;
    ViewBag.RoleId = role.Roleid;
    
    return View(permissions);
}

[HttpPost]
[Authorize(policy: "AdminOnly")]
public IActionResult UpdatePermissions(int roleId, [FromBody] List<PermissionUpdateModel> permissions)
{
    try
    {
        var existingPermissions = _dbcontext.Roleandpermissions
            .Where(rp => rp.Roleid == roleId)
            .ToList();

        _dbcontext.Roleandpermissions.RemoveRange(existingPermissions);

        foreach (var perm in permissions)
        {
            _dbcontext.Roleandpermissions.Add(new Roleandpermission
            {
                Roleid = roleId,
                Permissionid = perm.PermissionId,
                Canview = perm.CanView,
                Canaddedit = perm.CanAddEdit,
                Candelete = perm.CanDelete,
                Createddate = DateTime.Now,
            });
        }
        _dbcontext.SaveChanges();
        return Ok(new { success = true });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating permissions");
        return StatusCode(500, new { success = false });
    }
}
}
