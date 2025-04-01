using Entity.Models;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Userlogin> GetUserByEmailAsync(string email)
        {
            return await _context.Userlogins
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsCorrectPasswordAsync(string email, string password)
        {
            var user = await _context.Userlogins
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            return user != null && password == user.Password;
        }

        public async Task<int> GetUserIdByEmailAsync(string email)
        {
            var user = await _context.Userlogins
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            return user?.Userid ?? 0;
        }

        public async Task<string> GetUserNameByEmailAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null) return string.Empty;

            var userDetail = await _context.Userdetails
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Userid == user.Userid);

            return userDetail?.Username ?? string.Empty;
        }

        public async Task<int> GetRoleIdByEmailAsync(string email)
        {
            var user = await _context.Userlogins
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            return user?.Roleid ?? 0;
        }

        public async Task UpdateUserLoginAsync(Userlogin user)
        {
            _context.Userlogins.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<UserTable> Users, int TotalItems)> GetUsersAsync(
    string searchTerm,
    int page,
    int pageSize,
    string sortField = "Name", string sortDirection = "asc")
        {
            var query = _context.Userdetails
                .Include(ud => ud.User)
                .Include(ud => ud.Role)
                .Where(ud => !ud.Isdeleted)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                searchTerm = await RemoveWhitespace(searchTerm);
                query = query.Where(ud =>
                    (ud.Firstname + " " + ud.Lastname).ToLower().Contains(searchTerm) ||
                    ud.User.Email.ToLower().Contains(searchTerm) ||
                    ud.Role.Rolename.ToLower().Contains(searchTerm)
                );
            }

            query = sortField switch
            {
                "Name" => sortDirection == "asc"
                    ? query.OrderBy(ud => ud.Firstname).ThenBy(ud => ud.Lastname)
                    : query.OrderByDescending(ud => ud.Firstname).ThenByDescending(ud => ud.Lastname),
                "Role" => sortDirection == "asc"
                    ? query.OrderBy(ud => ud.Role.Rolename)
                    : query.OrderByDescending(ud => ud.Role.Rolename),
                _ => query.OrderBy(ud => ud.Userid)
            };

            var totalItems = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ud => new UserTable
                {
                    UserId = ud.Userid,
                    Name = $"{ud.Firstname} {ud.Lastname}",
                    Email = ud.User.Email,
                    Phone = ud.Phonenumber,
                    Role = ud.Role.Rolename,
                    Status = ud.Status ? "active" : "inactive",
                    ProfileImage = ud.Profileimage
                })
                .ToListAsync();

            return (users, totalItems);
        }

        public async Task<bool> IsUserExistsAsync(string email)
        {
            return await _context.Userlogins
                .AsNoTracking()
                .AnyAsync(u => u.Email == email);
        }

        public async Task AddNewUserAsync(AddUserDetail model)
        {
            var nextUserId = await _context.Userdetails.CountAsync() + 1;
            var now = DateTime.Now;

            var userlogin = new Userlogin
            {
                Userid = nextUserId,
                Email = model.Email,
                Password = model.Password,
                Roleid = model.Roleid,
                Isdeleted = false
            };

            var userdetail = new Userdetail
            {
                Userid = nextUserId,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Username = model.Username,
                Address = model.Address,
                Cityid = model.Cityid,
                Countryid = model.Countryid,
                Stateid = model.Stateid,
                Roleid = model.Roleid,
                Zipcode = model.Zipcode,
                Profileimage = model.ProfileimagePath,
                Phonenumber = model.Phonenumber,
                Status = true,
                Createddate = now,
                Isdeleted = false
            };

            await _context.Userlogins.AddAsync(userlogin);
            await _context.Userdetails.AddAsync(userdetail);
            await _context.SaveChangesAsync();
        }

        public async Task<Userdetail> GetUserByIdAsync(int userId)
        {
            return await _context.Userdetails
                .Include(ud => ud.User)
                .Include(ud => ud.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(ud => ud.Userid == userId);
        }

        public async Task<Userlogin> GetUserloginDetailsAsync(int userId)
        {
            return await _context.Userlogins
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Userid == userId);
        }

        public async Task UpdateUserDetailAsync(EditUserDetail model, string profileImagePath)
        {
            var user = await _context.Userdetails.FirstOrDefaultAsync(u=> u.Userid==model.UserId);
            var userlogin = await _context.Userlogins.FirstOrDefaultAsync(u=> u.Userid==model.UserId);
            userlogin.Roleid=model.Roleid;

            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Username = model.Username;
            user.Address = model.Address;
            user.Countryid = model.Countryid;
            user.Roleid = model.Roleid;
            user.Stateid = model.Stateid;
            user.Cityid = model.Cityid;
            user.Zipcode = model.Zipcode;
            user.Phonenumber = model.Phonenumber;
            user.Profileimage = profileImagePath ?? user.Profileimage;
            user.Status = model.Status == "1";

            await _context.SaveChangesAsync();
        }
        public async Task UpdateUserProfileAsync(MyProfile model, string profileImagePath)
        {
            var user = await GetUserByIdAsync(model.UserId);

            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Username = model.Username;
            user.Address = model.Address;
            user.Countryid = model.Countryid;
            user.Stateid = model.Stateid;
            user.Cityid = model.Cityid;
            user.Zipcode = model.Zipcode;
            user.Phonenumber = model.Phonenumber;
            user.Profileimage = profileImagePath ?? user.Profileimage;
            user.Status = model.Status == "1";
            user.Modifieddate = DateTime.Now;

            _context.Userdetails.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteUserByIdAsync(int id)
        {
            var userDetail = await _context.Userdetails
                .FirstOrDefaultAsync(u => u.Userid == id);
            var userLogin = await _context.Userlogins
                .FirstOrDefaultAsync(u => u.Userid == id);

            if (userDetail != null && userLogin != null)
            {
                userDetail.Isdeleted = true;
                userLogin.Isdeleted = true;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            return await _context.Countries
                .Where(c => !c.Isdeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId)
        {
            return await _context.States
                .Where(s => s.Countryid == countryId && !s.Isdeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<City>> GetCitiesByStateAsync(int stateId)
        {
            return await _context.Cities
                .Where(c => c.Stateid == stateId && !c.Isdeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Userrole>> GetRolesAsync()
        {
            return await _context.Userroles
                .Where(r => !r.Isdeleted)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<string> RemoveWhitespace(string input)
        {
            return new string(input
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }

}