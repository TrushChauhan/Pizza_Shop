using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
namespace Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View("Orders");
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderFilterModel filters)
        {
            try
            {
                var (orders, totalCount) = await _orderService.GetOrders(filters);
                return Ok(new { data = orders, totalCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] OrderFilterModel filters)
        {
            try
            {
                var excelBytes = await _orderService.ExportOrdersToExcel(filters);
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrdersExport.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            try
            {
                var orderDetails = await _orderService.GetOrderDetails(orderId);
                return View("OrderDetails", orderDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPdf(int id)
        {
            try
            {
                var pdfBytes = await _orderService.GenerateOrderPdf(
                    id,
                    ControllerContext,
                    ViewData,
                    TempData);

                var orderDetails = await _orderService.GetOrderDetails(id);
                return File(pdfBytes, "application/pdf", $"Invoice_{orderDetails.InvoiceNumber}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error generating PDF");
            }
        }
    }
}
