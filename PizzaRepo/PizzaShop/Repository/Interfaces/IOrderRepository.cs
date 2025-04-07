using Entity.Models;

namespace Repository.Interfaces;

public interface IOrderRepository
{
    IQueryable<Customerorder> GetAll();
}
