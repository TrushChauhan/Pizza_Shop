using Entity.Models;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Implementations;

public class UserService : IUserService
{   
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepo;

    private readonly IAuthService _authService;
    public UserService(IUserRepository userRepo,IConfiguration config,IAuthService authService)
    {
        _authService=authService;
        _userRepo = userRepo;
        _config = config;
    }
    public (List<UserTable> Users, int TotalItems) GetUsers(string SearchTerm,int page,int PageSize){
        var query=_userRepo.GetUsers(SearchTerm,page,PageSize);
        return query;
    }
    public void AddNewUser(AddUserDetail user){
        user.Password=_authService.EncryptPassword(user.Password);
        _userRepo.AddNewUser(user);
    }
    public Task<IEnumerable<Country>> GetCountriesAsync()
    {
        return _userRepo.GetCountriesAsync();
    }

    public Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId)
    {
        return _userRepo.GetStatesByCountryAsync(countryId);
    }

    public Task<IEnumerable<City>> GetCitiesByStateAsync(int stateId)
    {
        return _userRepo.GetCitiesByStateAsync(stateId);
    }
    public Task<IEnumerable<Userrole>> GetRolesAsync()
    {
        return _userRepo.GetRolesAsync();
    }
}
