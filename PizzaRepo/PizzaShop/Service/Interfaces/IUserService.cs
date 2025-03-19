using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface IUserService
{
    (List<UserTable> Users, int TotalItems) GetUsers(string searchTerm, int page,int pageSize);
    void AddNewUser(AddUserDetail user);
    EditUserDetail GetUserForEdit(int userId);
    void UpdateUser(EditUserDetail model, string profileImagePath);
    Task<IEnumerable<Country>> GetCountriesAsync();
    Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId);
    Task<IEnumerable<City>> GetCitiesByStateAsync(int stateId);
    List<Userrole> GetRoles();

    bool DeleteUser(int id);
}
