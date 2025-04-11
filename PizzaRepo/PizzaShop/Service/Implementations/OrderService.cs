using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Repository.Interfaces;
using SelectPdf;
using Service.Interfaces;
using System.Drawing;

namespace Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(
            IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
    
        }

        public async Task<(List<OrderViewModel> Orders, int TotalCount)> GetOrders(OrderFilterModel filters)
        {
            var query = _orderRepository.GetAll()
                .Include(o => o.Customer)
                .Where(o => !o.Isdeleted);

            // Apply filters 
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                query = query.Where(o =>
                    o.Orderid.ToString().Contains(filters.SearchTerm) ||
                    o.Customer.Customername.Contains(filters.SearchTerm));
            }

            if (filters.Status != "All Status" && !string.IsNullOrEmpty(filters.Status))
            {
                query = query.Where(o => o.Status == filters.Status);
            }

            if (filters.FromDate.HasValue)
            {
                var fromDate = DateOnly.FromDateTime(filters.FromDate.Value);
                query = query.Where(o => o.Date >= fromDate);
            }

            if (filters.ToDate.HasValue)
            {
                DateOnly toDate = DateOnly.FromDateTime(filters.ToDate.Value);
                query = query.Where(o => o.Date <= toDate);
            }

            if (filters.TimePeriod != "All time" && !string.IsNullOrEmpty(filters.TimePeriod))
            {
                DateTime now = DateTime.Now;
                switch (filters.TimePeriod)
                {
                    case "Last 7 days":
                        var sevenDaysAgo = now.AddDays(-7);
                        query = query.Where(o => o.Createddate >= sevenDaysAgo);
                        break;
                    case "Last 30 days":
                        var thirtyDaysAgo = now.AddDays(-30);
                        query = query.Where(o => o.Createddate >= thirtyDaysAgo);
                        break;
                    case "Current Month":
                        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                        query = query.Where(o => o.Createddate >= firstDayOfMonth);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(filters.SortField))
            {
                switch (filters.SortField)
                {
                    case "Order":
                        query = filters.SortAscending
                            ? query.OrderBy(o => o.Orderid)
                            : query.OrderByDescending(o => o.Orderid);
                        break;
                    case "Date":
                        query = filters.SortAscending
                            ? query.OrderBy(o => o.Date)
                            : query.OrderByDescending(o => o.Date);
                        break;
                    case "Customer":
                        query = filters.SortAscending
                            ? query.OrderBy(o => o.Customer.Customername)
                            : query.OrderByDescending(o => o.Customer.Customername);
                        break;
                    case "Total Amount":
                        query = filters.SortAscending
                            ? query.OrderBy(o => o.Totalamount)
                            : query.OrderByDescending(o => o.Totalamount);
                        break;
                }
            }

            int totalCount = await query.CountAsync();

            var pagedQuery = query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize);

            var orders = await pagedQuery.ToListAsync();
            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                OrderId = o.Orderid,
                Date = o.Date.ToDateTime(TimeOnly.MinValue),
                CustomerName = o.Customer.Customername,
                Status = o.Status,
                PaymentMode = o.Paymentmode,
                Rating = o.Rating,
                TotalAmount = o.Totalamount
            }).ToList();

            return (orderViewModels, totalCount);
        }

        public async Task<OrderDetailsViewModel> GetOrderDetails(int orderId)
        {
            var order = await _orderRepository.GetOrderWithDetails(orderId);
            if (order == null) return null;

            var invoice = order.Invoices.FirstOrDefault();

            return new OrderDetailsViewModel
            {
                OrderId = order.Orderid,
                InvoiceNumber = invoice != null ? $"#{invoice.Invoiceid}" : "N/A",
                Status = order.Status,
                PaymentMode = order.Paymentmode,
                PaidOn = invoice?.Createddate ?? order.Createddate,
                PlacedOn = order.Createddate,
                ModifiedOn = order.Modifieddate,
                OrderDuration = (invoice?.Createddate ?? DateTime.Now) - order.Createddate,
                TotalAmount = order.Totalamount,
                SubTotal = order.Orderdetails.Sum(od => od.Quantity * od.Item.Rate),
                CustomerName = order.Customer?.Customername,
                Phone = order.Customer?.Phonenumber,
                Email = order.Customer?.Email,
                NumberOfPersons = order.Ordertables.FirstOrDefault()?.Table?.Capacity ?? 0,
                TableName = order.Ordertables.FirstOrDefault()?.Table?.Tablename ?? "N/A",
                SectionName = order.Ordertables.FirstOrDefault()?.Table?.Section?.Sectionname ?? "N/A",
                OrderItems = order.Orderdetails.Select((od, index) => new OrderItemViewModel
                {
                    SrNo = index + 1,
                    ItemName = od.Item.Itemname,
                    Quantity = od.Quantity,
                    Price = od.Item.Rate,
                    TotalAmount = od.Quantity * od.Item.Rate,
                    Modifiers = od.Orderdetailmodifiers.Select(odm => new OrderItemModifierViewModel
                    {
                        ModifierName = odm.Modifier?.Modifiername,
                        Quantity = 1,
                        Price = odm.Modifier?.Rate ?? 0,
                        TotalAmount = odm.Modifier?.Rate ?? 0
                    }).ToList()
                }).ToList(),
                Taxes = order.Ordertaxes.Select(ot => new OrderTaxViewModel
                {
                    TaxName = ot.Tax?.Taxname,
                    TaxValue = ot.Taxvalue,
                }).ToList()
            };
        }

        public async Task<byte[]> GenerateOrderPdf(
    int orderId,
    ControllerContext controllerContext,
    ViewDataDictionary viewData,
    ITempDataDictionary tempData)
        {
            var orderDetails = await GetOrderDetails(orderId);
            if (orderDetails == null) return null;

            // Render the view to HTML string
            var htmlContent = await controllerContext.RenderViewAsync(
                "OrderDetailPdf",
                orderDetails,
                viewData,
                tempData);

            // Configure PDF converter
            var converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 10;
            converter.Options.MarginBottom = 10;

            // Convert HTML to PDF
            var pdfDocument = converter.ConvertHtmlString(htmlContent);

            // Save to memory stream
            using var memoryStream = new MemoryStream();
            pdfDocument.Save(memoryStream);
            pdfDocument.Close();

            return memoryStream.ToArray();
        }
        public async Task<byte[]> ExportOrdersToExcel(OrderFilterModel filters)
        {
            filters.PageNumber = 1;
            filters.PageSize = int.MaxValue;

            var (orders, _) = await GetOrders(filters);

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
            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 20;
            worksheet.Column(4).Width = 30;
            worksheet.Column(5).Width = 30;
            worksheet.Column(6).Width = 10;
            worksheet.Column(7).Width = 10;

            //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            return package.GetAsByteArray();
        }

    }
}