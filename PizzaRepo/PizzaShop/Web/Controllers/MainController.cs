using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Entity.ViewModel;
using Service.Interfaces;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Web.Filters;

namespace Web.Controllers
{
    public class MainController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IRolesAndPermissionsService _rolesPermissionsService;
        private readonly IFileService _fileService;
        private readonly INotyfService _notify;

        public MainController(
            IUserService userService,
            IConfiguration config,
            IAuthService authService,
            IEmailService emailService,
            IRolesAndPermissionsService rolesAndPermissionsService,
            IFileService fileService,
            INotyfService notyfService)
        {
            _config = config;
            _userService = userService;
            _authService = authService;
            _emailService = emailService;
            _rolesPermissionsService = rolesAndPermissionsService;
            _fileService = fileService;
            _notify = notyfService;
        }

        [Authorize]
        public async Task<IActionResult> GetUserName()
        {
            string email = User.FindFirst(ClaimTypes.Name)?.Value;
            int userId = await _authService.GetUserIdByEmailAsync(email);
            var user = await _userService.GetUserForEditAsync(userId);
            return Json(new { name = user.Username, imagePath = user.ExistingProfileImage });
        }

        public async Task<IActionResult> MyProfile()
        {
            string email = User.FindFirst(ClaimTypes.Name)?.Value;
            int userId = await _authService.GetUserIdByEmailAsync(email);
            var profile = await _userService.GetProfileForUpdateAsync(userId);
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(MyProfile model)
        {
            try
            {
                var imagePath = model.ExistingProfileImage;
                if (model.ProfileImageFile != null)
                {

                    imagePath = await _fileService.SaveProfileImageAsync(model.ProfileImageFile);
                }

                await _userService.UpdateUserProfileAsync(model, imagePath);
                _notify.Custom("Profile Updated Successfully", 5, "Green", "fa-solid fa-check");
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating user: " + ex.Message);
                return View(model);
            }
        }
        [PermissionAuthorize("Users", "view")]
        public async Task<IActionResult> Users(
    string searchTerm = null,
    int page = 1,
    int pageSize = 5,
    string sortField = "Name",
    string sortDirection = "asc")
        {
            var (users, totalItems) = await _userService.GetUsersAsync(
                searchTerm,
                page,
                pageSize,
                sortField,
                sortDirection);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortField = sortField;
            ViewBag.SortDirection = sortDirection;

            return View(users);
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
        public async Task<IActionResult> DoChangePassword(ChangePasswordModel model)
        {
            string userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return await LogoutAsync();
            }

            bool isCorrectPassword = await _authService.IsCorrectPasswordAsync(userEmail, model.CurrentPassword);
            if (!isCorrectPassword)
            {
                TempData["Message"] = "Invalid Password";
                return RedirectToAction("ChangePassword", "Main");
            }

            await _authService.ResetPasswordAsync(userEmail, model.ConfirmNewPassword);
            return RedirectToAction("Index", "Home");
        }

        [PermissionAuthorize("Users", "edit")]
        public IActionResult AddNewUser()
        {
            return View();
        }

        [HttpPost]
        [PermissionAuthorize("Users", "edit")]
        public async Task<IActionResult> AddNewUser(AddUserDetail model)
        {
            try
            {
                await _emailService.SendEmailToNewUserAsync(model.Email, model.Password);
                await _userService.AddNewUserAsync(model);
                _notify.Custom("User Added Successfully", 5, "green", "fa-solid fa-check");
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error adding user: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        [PermissionAuthorize("Users", "edit")]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userService.GetUserForEditAsync(id);
            return View(user);
        }

        [HttpPost]
        [PermissionAuthorize("Users", "edit")]
        public async Task<IActionResult> EditUser(EditUserDetail model)
        {
            try
            {
                var imagePath = model.ExistingProfileImage;
                if (model.ProfileImageFile != null)
                {
                    imagePath = await _fileService.SaveProfileImageAsync(model.ProfileImageFile);
                }

                await _userService.UpdateUserAsync(model, imagePath);
                _notify.Custom("User Updated Successfully", 5, "Green", "fa-solid fa-check");
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating user: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [PermissionAuthorize("Users", "delete")]
        public async Task<IActionResult> Delete(int id)
        {
            bool isDeleted = await _userService.DeleteUserAsync(id);
            return isDeleted
                ? Ok(new { success = true, message = "User deleted successfully" })
                : NotFound(new { success = false, message = "User not found" });
        }

        [HttpGet]
        public async Task<IActionResult> LoadCountry()
        {
            var countries = await _userService.GetCountriesAsync();
            return Ok(countries.Select(c => new { countryid = c.Countryid, name = c.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> GetState(int countryId)
        {
            var states = await _userService.GetStatesByCountryAsync(countryId);
            return Ok(states.Select(s => new { stateid = s.Stateid, name = s.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> GetCity(int stateId)
        {
            var cities = await _userService.GetCitiesByStateAsync(stateId);
            return Ok(cities.Select(c => new { cityid = c.Cityid, name = c.Name }));
        }

        [PermissionAuthorize("dashboard", "view")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [PermissionAuthorize("RolesAndPermissions", "view")]
        public async Task<IActionResult> Roles()
        {
            var roles = await _userService.GetRolesAsync();
            return View(roles);
        }

        [PermissionAuthorize("RolesAndPermissions", "view")]
        public async Task<IActionResult> Permissions(int roleId)
        {
            var permissions = await _rolesPermissionsService.GetPermissionsByRoleAsync(roleId);
            var role = await _authService.GetRoleByIdAsync(roleId);

            ViewBag.RoleName = role;
            ViewBag.RoleId = roleId;

            return View(permissions);
        }

        [HttpPost]
        [PermissionAuthorize("RolesAndPermissions", "edit")]
        public async Task<IActionResult> UpdatePermissions(int roleId, [FromBody] List<PermissionUpdateModel> permissions)
        {
            try{
            await _rolesPermissionsService.UpdatePermissionsAsync(roleId, permissions);
            _notify.Custom("Permissions Updated Successfully", 5, "Green", "fa-solid fa-check");
            return Ok(new { success = true });
            }
            catch{
                return BadRequest();
            }
        }

        private async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}