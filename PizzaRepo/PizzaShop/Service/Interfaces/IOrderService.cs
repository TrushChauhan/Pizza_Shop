using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface IOrderService
{
    Task<(List<Customerorder> Orders, int TotalCount)> GetOrders(OrderFilterModel filters);
}
