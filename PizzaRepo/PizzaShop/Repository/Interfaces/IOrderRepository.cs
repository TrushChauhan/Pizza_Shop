using Entity.Models;

namespace Repository.Interfaces;

public interface IOrderRepository
{
    IQueryable<Customerorder> GetAll();
    Task<Customerorder> GetOrderWithDetails(int orderId);

}
