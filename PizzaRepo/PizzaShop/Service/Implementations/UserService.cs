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
    private readonly IFileService _fileService;
    private readonly IAuthService _authService;
    public UserService(IUserRepository userRepo,IConfiguration config,IAuthService authService,IFileService fileService)
    {
        _authService=authService;
        _userRepo = userRepo;
        _config = config;
        _fileService=fileService;
    }
    public (List<UserTable> Users, int TotalItems) GetUsers(string SearchTerm,int page,int PageSize){
        var query=_userRepo.GetUsers(SearchTerm,page,PageSize);
        return query;
    }
    public void AddNewUser(AddUserDetail user){
        var imagePath = _fileService.SaveProfileImage(user.ProfileImageFile).Result;
        user.ProfileimagePath=imagePath;
        user.Password=_authService.EncryptPassword(user.Password);
        _userRepo.AddNewUser(user);
    }
    public bool DeleteUser(int id){
        return _userRepo.DeleteUserById(id);
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
    public List<Userrole> GetRoles()
    {
        return _userRepo.GetRoles();
    }
}

