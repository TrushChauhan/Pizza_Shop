using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IOrderService
    {
        Task<(List<OrderViewModel> Orders, int TotalCount)> GetOrders(OrderFilterModel filters);
        Task<OrderDetailsViewModel> GetOrderDetails(int orderId);
        Task<byte[]> GenerateOrderPdf(int orderId, ControllerContext controllerContext, ViewDataDictionary viewData, ITempDataDictionary tempData);
        Task<byte[]> ExportOrdersToExcel(OrderFilterModel filters);
    }
}