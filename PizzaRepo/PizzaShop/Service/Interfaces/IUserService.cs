using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface IUserService
{
    Task<(List<UserTable> Users, int TotalItems)> GetUsersAsync(string searchTerm, int page, int pageSize,string sortField,string sortDirection);
    Task<MyProfile> GetProfileForUpdateAsync(int userId);
    Task UpdateUserProfileAsync(MyProfile model, string profileImagePath);
    Task AddNewUserAsync(AddUserDetail user);
    Task<EditUserDetail> GetUserForEditAsync(int userId);
    Task UpdateUserAsync(EditUserDetail model, string profileImagePath);
    Task<bool> DeleteUserAsync(int id);
    Task<IEnumerable<Country>> GetCountriesAsync();
    Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId);
    Task<IEnumerable<City>> GetCitiesByStateAsync(int stateId);
    Task<List<Userrole>> GetRolesAsync();
}
