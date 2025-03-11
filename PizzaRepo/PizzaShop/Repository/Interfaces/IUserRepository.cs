using Entity.Models;
namespace Repository.Interfaces;

public interface IUserRepository
{
   public Userlogin GetUserByEmail(string email);
    void UpdateUser(Userlogin user);
   bool UserExists(string email);

   int GetRoleIdByEmail(string email);
}

