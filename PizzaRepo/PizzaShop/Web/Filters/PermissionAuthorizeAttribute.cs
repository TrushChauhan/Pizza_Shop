using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Filters
{
    public class PermissionAuthorizeAttribute : TypeFilterAttribute
    {
        public PermissionAuthorizeAttribute(string module, string operation) 
            : base(typeof(PermissionAuthorizeFilter))
        {
            Arguments = new object[] { module, operation };
        }
    }

    public class PermissionAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly string _module;
        private readonly string _operation;
        private readonly IRolesAndPermissionsService _permsService;
        private readonly IRoleRepository _roleRepo;

        public PermissionAuthorizeFilter(
            string module,
            string operation,
            IRolesAndPermissionsService permsService,
            IRoleRepository roleRepo)
        {
            _module = module;
            _operation = operation;
            _permsService = permsService;
            _roleRepo = roleRepo;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            
            // 1. Check authentication
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            // 2. Get role from claims
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            if (roleClaim == null)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            // 3. Get fresh role details from DB
            var role = await _roleRepo.GetRoleByNameAsync(roleClaim.Value);
            if (role == null)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            // 4. Get fresh permissions
            var permissions = await _permsService.GetPermissionsByRoleAsync(role.Roleid);
            var permission = permissions.FirstOrDefault(p => p.PermissionName == _module);

            // 5. Check permission
            if (!HasPermission(permission))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }

        private bool HasPermission(PermissionViewModel permission)
        {
            if (permission == null) return false;

            return _operation.ToLower() switch
            {
                "view" => permission.CanView,
                "edit" => permission.CanAddEdit,
                "delete" => permission.CanDelete,
                _ => false
            };
        }
    }
}