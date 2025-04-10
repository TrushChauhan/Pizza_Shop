using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SelectPdf;
using System.IO;
using System.Threading.Tasks;

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

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Orders");

            var headers = new string[]
            {
        "Order ID", "Date", "Customer", "Status",
        "Payment Mode", "Rating", "Total Amount"
            };

            void SetInfoCell(string title, string value, int startRow, int startCol)
            {
                var titleCell = worksheet.Cells[startRow, startCol, startRow + 1, startCol + 1];
                titleCell.Merge = true;
                titleCell.Value = title;
                titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                titleCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                titleCell.Style.Font.Color.SetColor(Color.White);
                titleCell.Style.Font.Bold = true;
                titleCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                titleCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var valueCell = worksheet.Cells[startRow, startCol + 2, startRow + 1, startCol + 5];
                valueCell.Merge = true;
                valueCell.Value = value;
                valueCell.Style.Font.Bold = true;
                valueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                valueCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Add logo
            using (FileStream stream = new FileStream(@"D:\Project\Pizza_Shop\PizzaShopProject\images\logos\pizzashop_logo.png", FileMode.Open))
            {
                ExcelPicture excelImage = worksheet.Drawings.AddPicture("logo", stream);
                excelImage.SetPosition(2, 0, 12, 0);
                excelImage.SetSize(100, 100);
            }

            // Add info section
            SetInfoCell("Status Filter:", filters.Status ?? "All Status", 1, 1);
            SetInfoCell("Search Text:", filters.SearchTerm ?? "None", 1, 7);
            SetInfoCell("Date Range:", filters.FromDate.HasValue && filters.ToDate.HasValue
                ? $"{filters.FromDate.Value:yyyy-MM-dd} to {filters.ToDate.Value:yyyy-MM-dd}"
                : filters.TimePeriod ?? "All time", 4, 1);
            SetInfoCell("Number of Records:", orders.Count.ToString(), 4, 7);

            // Add headers at row 9
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[9, i + 1].Value = headers[i];
                var cell = worksheet.Cells[9, i + 1];
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.Font.Bold = true;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Fill order data starting from row 10
            int row = 10;
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

            // Align all data cells
            for (int i = 1; i <= orders.Count + 9; i++)
            {
                for (int j = 1; j <= headers.Length + 6; j++)
                {
                    worksheet.Cells[i, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                }
            }
            worksheet.Column(1).Width = 2;
            worksheet.Column(2).Width = 4;
            worksheet.Column(3).Width = 3;
            worksheet.Column(4).Width = 3;
            worksheet.Column(5).Width = 2;
            worksheet.Column(6).Width = 2;
            worksheet.Column(7).Width = 2;

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var fileName = $"OrdersExport.xlsx";
            return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            try
            {
                OrderDetailsViewModel orderDetails = await _orderService.GetOrderDetails(orderId);
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
            // Fetch order details
            var orderDetails = await _orderService.GetOrderDetails(id);

            // Render the view to HTML string
            var viewHtml = await this.RenderViewAsync("OrderDetailPdf", orderDetails);

            // Instantiate the HTML to PDF converter
            var converter = new HtmlToPdf();

            // Configure converter options
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 10;
            converter.Options.MarginBottom = 10;

            // Convert HTML to PDF
            var pdfDocument = converter.ConvertHtmlString(viewHtml);

            // Save the PDF to a memory stream
            var pdfStream = new MemoryStream();
            pdfDocument.Save(pdfStream);
            pdfStream.Position = 0; 

            // Close the PDF document
            pdfDocument.Close();

            // Return PDF file
            return File(pdfStream, "application/pdf", $"Invoice_{orderDetails.InvoiceNumber}.pdf");
        }
    }
}
