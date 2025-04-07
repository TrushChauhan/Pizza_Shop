using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface IOrderService
{
    List<Customerorder> GetOrders(OrderFilterModel filters);
}
