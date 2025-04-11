using Entity.Models;
using Repository.Interfaces;

namespace Repository.Implementations;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;
    public CustomerRepository(ApplicationDbContext context){
        _context=context;
    }
    
}
