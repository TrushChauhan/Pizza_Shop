using Entity.Models;

namespace Repository.Interfaces
{
    public interface ICustomerRepository
    {
        IQueryable<Customer> GetAll();
    }
}