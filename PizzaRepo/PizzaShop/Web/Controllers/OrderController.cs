using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using OfficeOpenXml.Drawing;
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

            filters.PageNumber = 1;
            filters.PageSize = int.MaxValue;

            var (orders, _) = await _orderService.GetOrders(filters);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Orders");

            var headers = new string[]
            {
        "Order ID", "Date", "Customer", "Status",
        "Payment Mode", "Rating", "Total Amount"
            };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[9, i + 1].Value = headers[i];
                worksheet.Cells[9, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                worksheet.Cells[9, i + 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[9, i + 1].Style.Font.Bold = true;
            }

            int row = 10;
            int infoRow = 1;

            worksheet.Cells[infoRow + 1, 1].Value = "Status Filter:";
            worksheet.Cells[infoRow + 1, 3].Value = filters.Status ?? "All Status";

            worksheet.Cells[infoRow + 1, 6].Value = "Search Text:";
            worksheet.Cells[infoRow + 1, 8].Value = filters.SearchTerm ?? "None";

            worksheet.Cells[infoRow + 4, 1].Value = "Date Range:";
            worksheet.Cells[infoRow + 4, 3].Value =
                filters.FromDate.HasValue && filters.ToDate.HasValue
                    ? $"{filters.FromDate.Value:yyyy-MM-dd} to {filters.ToDate.Value:yyyy-MM-dd}"
                    : filters.TimePeriod ?? "All time";
            worksheet.Cells[infoRow + 4, 6].Value = "Number of Records:";
            worksheet.Cells[infoRow + 4, 8].Value = orders.Count;
            foreach (var order in orders)
            {
                worksheet.Cells[row, 1].Value = order.OrderId;
                worksheet.Cells[row, 2].Value = order.Date;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "yyyy-mm-dd";
                worksheet.Cells[row, 3].Value = order.CustomerName;
                worksheet.Cells[row, 4].Value = order.Status;
                worksheet.Cells[row, 5].Value = order.PaymentMode;
                worksheet.Cells[row, 6].Value = order.Rating;
                worksheet.Cells[row, 7].Value = order.TotalAmount;
                row++;
            }
            using (FileStream stream = new FileStream(@"D:\Project\Pizza_Shop\PizzaShopProject\images\logos\pizzashop_logo.png", FileMode.Open))
            {
                ExcelPicture excelImage = worksheet.Drawings.AddPicture("logo", stream);
                excelImage.SetPosition(2, 0, 10, 0);
                excelImage.SetSize(100, 100);
            }
            for (int i = 1; i <= orders.Count + 9; i++)
            {
                for (int j = 1; j <= headers.Length + 4; j++)
                {
                    worksheet.Cells[i, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                }
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var fileName = $"OrdersExport.xlsx";

            return File(
                package.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
    }
}
