using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface IOrderService
{
    Task<List<Customerorder>> GetOrders(OrderFilterModel filters);
}
