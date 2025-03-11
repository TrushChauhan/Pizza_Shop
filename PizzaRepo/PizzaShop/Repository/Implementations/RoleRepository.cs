using Entity.Models;
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
}
