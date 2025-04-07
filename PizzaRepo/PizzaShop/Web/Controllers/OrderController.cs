using System.Security.Cryptography.X509Certificates;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Web.Controllers{
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
public IActionResult GetOrders([FromQuery] OrderFilterModel filters)
{
    try
    {
        var orders = _orderService.GetOrders(filters);
        
        // Convert to ViewModel
        var orderViewModels = orders.Select(o => new OrderViewModel
        {
            OrderId = o.Orderid,
            Date = o.Date.ToDateTime(TimeOnly.MinValue),
            CustomerName = o.Customer?.Customername ?? "Unknown",
            Status = o.Status, // Directly use the string status now
            PaymentMode = o.Paymentmode,
            Rating = o.Rating,
            TotalAmount = o.Totalamount
        }).ToList();

        return Ok(orderViewModels);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = ex.Message });
    }
}
}
}
