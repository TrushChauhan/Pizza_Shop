using Entity.ViewModel;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Implementations;

public class RolesAndPermissionsService : IRolesAndPermissionsService
{
    private readonly IRoleRepository _roleRepo;
    public RolesAndPermissionsService(IRoleRepository roleRepo)
    {
        _roleRepo = roleRepo;
    }
    public async Task<List<PermissionViewModel>>  GetPermissionsByRoleAsync(int roleId){
        return await _roleRepo.GetPermissionsByRoleAsync(roleId);
    }
    public async Task UpdatePermissionsAsync(int roleId, List<PermissionUpdateModel> permissions){
       await _roleRepo.UpdatePermissionsAsync(roleId,permissions);
    }
}
