using Entity.Models;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public string GetRoleById(int id){
        var userRole = _context.Userroles
                .FirstOrDefault(u => u.Roleid == id);
        return userRole.Rolename;
    }
    public List<PermissionViewModel>  GetPermissionsByRole(int roleId){
        var role = _context.Userroles
        .Include(r => r.Roleandpermissions)
        .FirstOrDefault(r => r.Roleid == roleId && !r.Isdeleted);

    var rolePermissions = role.Roleandpermissions
        .ToDictionary(rp => rp.Permissionid);

    var permissions = _context.Userpermissions
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
        return permissions;
    }
   public void UpdatePermissions(int roleId, List<PermissionUpdateModel> permissions){
        var existingPermissions = _context.Roleandpermissions
            .Where(rp => rp.Roleid == roleId)
            .ToList();

        _context.Roleandpermissions.RemoveRange(existingPermissions);

        foreach (var perm in permissions)
        {
            _context.Roleandpermissions.Add(new Roleandpermission
            {
                Roleid = roleId,
                Permissionid = perm.PermissionId,
                Canview = perm.CanView,
                Canaddedit = perm.CanAddEdit,
                Candelete = perm.CanDelete,
                Createddate = DateTime.Now,
            });
        }
        _context.SaveChanges();

   }

}
