using Entity.Models;
using Repository.Interfaces;

namespace Repository.Implementations;

public class OrderRepository:IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Customerorder> GetAll()
    {
        return _context.Customerorders.AsQueryable();
    }
}
