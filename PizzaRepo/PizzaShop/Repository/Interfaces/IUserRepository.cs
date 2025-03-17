using Entity.Models;
using Entity.ViewModel;
namespace Repository.Interfaces;

public interface IUserRepository
{
   public Userlogin GetUserByEmail(string email);
   void UpdateUser(Userlogin user);
   bool IsUserExists(string email);

   int GetRoleIdByEmail(string email);
   bool IsCorrectPassword(string email,string Password);
   (List<UserTable> Users, int TotalItems) GetUsers(string searchTerm, int page, int pageSize);

   void AddNewUser(AddUserDetail userDetail);
    Task<IEnumerable<Country>> GetCountriesAsync();
    Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId);
    Task<IEnumerable<City>> GetCitiesByStateAsync(int stateId);
    Task<IEnumerable<Userrole>> GetRolesAsync();
}

