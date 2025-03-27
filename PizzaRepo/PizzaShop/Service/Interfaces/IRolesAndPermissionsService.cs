using Entity.ViewModel;

namespace Service.Interfaces;

public interface IRolesAndPermissionsService
{
  Task<List<PermissionViewModel>>  GetPermissionsByRoleAsync(int roleId);
  Task UpdatePermissionsAsync(int roleId, List<PermissionUpdateModel> permissions);
}
