using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface IOrderService
{
    Task<(List<OrderViewModel> Orders, int TotalCount)> GetOrders(OrderFilterModel filters);
    Task<OrderDetailsViewModel> GetOrderDetails(int orderId);
}
