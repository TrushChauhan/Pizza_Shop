using Entity.Models;
using Entity.ViewModel;

namespace Repository.Interfaces;

public interface IRoleRepository
{
    public Task<string> GetRoleByIdAsync(int id);
    public Task<List<PermissionViewModel>>  GetPermissionsByRoleAsync(int roleId);
    public Task UpdatePermissionsAsync(int roleId, List<PermissionUpdateModel> permissions);
    Task<Userrole> GetRoleByNameAsync(string roleName);
}