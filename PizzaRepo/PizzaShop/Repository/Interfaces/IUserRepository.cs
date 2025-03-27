using Entity.Models;
using Entity.ViewModel;
namespace Repository.Interfaces;

public interface IUserRepository
{
    Task<Userlogin> GetUserByEmailAsync(string email);
    Task<bool> IsCorrectPasswordAsync(string email, string password);
    Task<int> GetUserIdByEmailAsync(string email);
    Task<string> GetUserNameByEmailAsync(string email);
    Task<int> GetRoleIdByEmailAsync(string email);
    Task UpdateUserLoginAsync(Userlogin user);
    Task<(List<UserTable> Users, int TotalItems)> GetUsersAsync(string searchTerm, int page, int pageSize,string sortField,string sortDirection);
    Task<bool> IsUserExistsAsync(string email);
    Task AddNewUserAsync(AddUserDetail model);
    Task<Userdetail> GetUserByIdAsync(int userId);
    Task<Userlogin> GetUserloginDetailsAsync(int userId);
    Task UpdateUserDetailAsync( EditUserDetail detail, string profileImagePath);
    Task UpdateUserProfileAsync(MyProfile model, string profileImagePath);
    Task<bool> DeleteUserByIdAsync(int id);
    Task<IEnumerable<Country>> GetCountriesAsync();
    Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId);
    Task<IEnumerable<City>> GetCitiesByStateAsync(int stateId);
    Task<List<Userrole>> GetRolesAsync();
}

