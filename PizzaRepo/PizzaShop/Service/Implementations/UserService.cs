using Entity.Models;
using Entity.ViewModel;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepo;
        private readonly IFileService _fileService;
        private readonly IAuthService _authService;

        public UserService(
            IUserRepository userRepo, 
            IConfiguration config, 
            IAuthService authService, 
            IFileService fileService)
        {
            _authService = authService;
            _userRepo = userRepo;
            _config = config;
            _fileService = fileService;
        }

        public async Task<(List<UserTable> Users, int TotalItems)> GetUsersAsync(string searchTerm, int page, int pageSize,string sortField,string sortDirection)
        {
            return await _userRepo.GetUsersAsync(searchTerm, page, pageSize,sortField,sortDirection);
        }

        public async Task<MyProfile> GetProfileForUpdateAsync(int userId)
        {
            var userDetail = await _userRepo.GetUserByIdAsync(userId);
            return new MyProfile
            {
                UserId = userDetail.Userid,
                Firstname = userDetail.Firstname,
                Lastname = userDetail.Lastname,
                Username = userDetail.Username,
                Email = userDetail.User.Email,
                Roleid = userDetail.Roleid,
                Countryid = userDetail.Countryid,
                Stateid = userDetail.Stateid,
                Cityid = userDetail.Cityid,
                Address = userDetail.Address,
                Zipcode = userDetail.Zipcode,
                Status = userDetail.Status ? "1" : "2",
                Phonenumber = userDetail.Phonenumber,
                ExistingProfileImage = userDetail.Profileimage
            };
        }

        public async Task UpdateUserProfileAsync(MyProfile model, string profileImagePath)
        {
            await _userRepo.UpdateUserProfileAsync(model,profileImagePath);
        }

        public async Task AddNewUserAsync(AddUserDetail user)
        {
            var imagePath = await _fileService.SaveProfileImageAsync(user.ProfileImageFile);
            user.ProfileimagePath = imagePath;
            user.Password = _authService.EncryptPassword(user.Password);
            await _userRepo.AddNewUserAsync(user);
        }

        public async Task<EditUserDetail> GetUserForEditAsync(int userId)
        {
            Userdetail userDetail = await _userRepo.GetUserByIdAsync(userId);
            return new EditUserDetail
            {
                UserId = userDetail.Userid,
                Firstname = userDetail.Firstname,
                Lastname = userDetail.Lastname,
                Username = userDetail.Username,
                Email = userDetail.User.Email,
                Roleid = userDetail.Roleid,
                Countryid = userDetail.Countryid,
                Stateid = userDetail.Stateid,
                Cityid = userDetail.Cityid,
                Address = userDetail.Address,
                Zipcode = userDetail.Zipcode,
                Status = userDetail.Status ? "1" : "2",
                Phonenumber = userDetail.Phonenumber,
                ExistingProfileImage = userDetail.Profileimage
            };
        }

        public async Task UpdateUserAsync(EditUserDetail model, string profileImagePath)
        {
            await _userRepo.UpdateUserDetailAsync(model,profileImagePath);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepo.DeleteUserByIdAsync(id);
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            return await _userRepo.GetCountriesAsync();
        }

        public async Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId)
        {
            return await _userRepo.GetStatesByCountryAsync(countryId);
        }

        public async Task<IEnumerable<City>> GetCitiesByStateAsync(int stateId)
        {
            return await _userRepo.GetCitiesByStateAsync(stateId);
        }

        public async Task<List<Userrole>> GetRolesAsync()
        {
            return await _userRepo.GetRolesAsync();
        }
    }
}