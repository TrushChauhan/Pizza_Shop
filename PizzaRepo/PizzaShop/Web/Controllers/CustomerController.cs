using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            return View("Customers");
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] CustomerFilterModel filters)
        {
            var (customerViewModels,totalCount) = await _customerService.GetCustomers(filters);
            return Ok(new { data = customerViewModels, totalCount });
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] CustomerFilterModel filters)
        {
            var excelBytes = await _customerService.ExportCustomersToExcelAsync(filters);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CustomersExport.xlsx"); 
        }
    }
}