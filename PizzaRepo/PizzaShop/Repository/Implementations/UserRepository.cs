namespace Repository.Implementations;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Userlogin GetUserByEmail(string email)
    {
        return _context.Userlogins
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Email == email);
    }
    public int GetRoleIdByEmail(string email){
        var user = _context.Userlogins
                .FirstOrDefault(u => u.Email ==email);
        return user.Roleid;
    }
    public void UpdateUser(Userlogin user)
    {
        _context.Userlogins.Update(user);
        _context.SaveChanges();
    }

    public bool IsUserExists(string email)
    {
        return _context.Userlogins.Any(u => u.Email == email);
    }
}