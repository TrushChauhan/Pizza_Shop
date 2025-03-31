using Entity.Models;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetRoleByIdAsync(int id)
        {
            var user = await _context.Userroles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Roleid == id);
            
            return user?.Rolename;
        }
        public async Task<Userrole> GetRoleByNameAsync(string roleName)
{
    return await _context.Userroles
        .AsNoTracking()
        .FirstOrDefaultAsync(r => r.Rolename == roleName && !r.Isdeleted);
}
        public async Task<List<PermissionViewModel>> GetPermissionsByRoleAsync(int roleId)
        {
            var role = await _context.Userroles
                .Include(r => r.Roleandpermissions)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Roleid == roleId && !r.Isdeleted);

            if (role == null)
            {
                return new List<PermissionViewModel>();
            }

            var rolePermissions = role.Roleandpermissions
                .ToDictionary(rp => rp.Permissionid);

            var permissions = await _context.Userpermissions
                .Where(p => !p.Isdeleted)
                .AsNoTracking()
                .ToListAsync();

            return permissions.Select(p =>
            {
                rolePermissions.TryGetValue(p.Permissionid, out var rp);
                return new PermissionViewModel
                {
                    PermissionId = p.Permissionid,
                    PermissionName = p.Permissionname,
                    CanView = rp?.Canview ?? true,
                    CanAddEdit = rp?.Canaddedit ?? true,
                    CanDelete = rp?.Candelete ?? true
                };
            }).ToList();
        }

        public async Task UpdatePermissionsAsync(int roleId, List<PermissionUpdateModel> permissions)
        {
            var existingPermissions = await _context.Roleandpermissions
                .Where(rp => rp.Roleid == roleId)
                .ToListAsync();

            foreach (var perm in permissions)
            {
                var existing = existingPermissions
                    .FirstOrDefault(rp => rp.Permissionid == perm.PermissionId);

                if (existing != null)
                {
                    existing.Canview = perm.CanView;
                    existing.Canaddedit = perm.CanAddEdit;
                    existing.Candelete = perm.CanDelete;
                }
                else
                {
                    // Consider adding new permissions if they don't exist
                    _context.Roleandpermissions.Add(new Roleandpermission
                    {
                        Roleid = roleId,
                        Permissionid = perm.PermissionId,
                        Canview = perm.CanView,
                        Canaddedit = perm.CanAddEdit,
                        Candelete = perm.CanDelete
                    });
                }
            }

            await _context.SaveChangesAsync();
            return;
        }
    }
}