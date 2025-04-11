using System.Drawing;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<(List<CustomerViewModel> Customers, int TotalCount)> GetCustomers(CustomerFilterModel filters)
        {
            var query = _customerRepository.GetAll();

            // Search filter
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                query = query.Where(c =>
                    c.Customername.Contains(filters.SearchTerm) ||
                    c.Email.Contains(filters.SearchTerm) ||
                    c.Phonenumber.Contains(filters.SearchTerm));
            }

            // Time period filter
            if (filters.TimePeriod != "All time" && !string.IsNullOrEmpty(filters.TimePeriod))
            {
                var now = DateOnly.FromDateTime(DateTime.Now);
                switch (filters.TimePeriod)
                {
                    case "Last 7 days":
                        var sevenDaysAgo = now.AddDays(-7);
                        query = query.Where(c => c.Date >= sevenDaysAgo);
                        break;
                    case "Last 30 days":
                        var thirtyDaysAgo = now.AddDays(-30);
                        query = query.Where(c => c.Date >= thirtyDaysAgo);
                        break;
                    case "Current Month":
                        var firstDayOfMonth = new DateOnly(now.Year, now.Month, 1);
                        query = query.Where(c => c.Date >= firstDayOfMonth);
                        break;
                }
            }

            // Date range filter
            if (filters.FromDate.HasValue)
            {
                query = query.Where(c => c.Date >= DateOnly.FromDateTime(filters.FromDate.Value));
            }

            if (filters.ToDate.HasValue)
            {
                query = query.Where(c => c.Date <= DateOnly.FromDateTime(filters.ToDate.Value));
            }

            if (!string.IsNullOrEmpty(filters.SortField))
            {
                switch (filters.SortField)
                {
                    case "Name":
                        query = filters.SortAscending
                            ? query.OrderBy(c => c.Customername)
                            : query.OrderByDescending(c => c.Customername);
                        break;
                    case "Date":
                        query = filters.SortAscending
                            ? query.OrderBy(c => c.Date)
                            : query.OrderByDescending(c => c.Date);
                        break;
                    case "Total Order":
                        query = filters.SortAscending
                            ? query.OrderBy(c => c.Totalorder)
                            : query.OrderByDescending(c => c.Totalorder);
                        break;
                }
            }

            int totalCount = await query.CountAsync();

            var customers = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            var customerViewModels = customers.Select(c => new CustomerViewModel
            {
                CustomerId = c.Customerid,
                Name = c.Customername,
                Email = c.Email,
                PhoneNumber = c.Phonenumber,
                Date = c.Date.ToDateTime(TimeOnly.MinValue),
                TotalOrders = c.Totalorder
            }).ToList();
            return (customerViewModels, totalCount);
        }
        public async Task<byte[]> ExportCustomersToExcelAsync(CustomerFilterModel filters)
        {
            filters.PageNumber = 1;
            filters.PageSize = int.MaxValue;

            var (customers, _) = await GetCustomers(filters);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Customers");

            var headers = new string[]
            {
        "Customer ID", "Name", "Email", "Date",
        "Phone Number", "Total Orders"
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
            SetInfoCell("Search Text:", filters.SearchTerm ?? "None", 1, 1);
            SetInfoCell("Filter Date Range:", filters.FromDate.HasValue && filters.ToDate.HasValue
                ? $"{filters.FromDate.Value:yyyy-MM-dd} to {filters.ToDate.Value:yyyy-MM-dd}"
                : filters.TimePeriod ?? "All time", 1, 7);
            SetInfoCell("Status Filter:", filters.SortField ?? "All", 4, 1);
            SetInfoCell("Number of Records:", customers.Count.ToString(), 4, 7);

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

            // Fill customer data starting from row 10
            int row = 10;
            foreach (var customer in customers)
            {
                worksheet.Cells[row, 1].Value = customer.CustomerId;
                worksheet.Cells[row, 2].Value = customer.Name;
                worksheet.Cells[row, 3].Value = customer.Email;
                worksheet.Cells[row, 4].Value = customer.Date;
                worksheet.Cells[row, 4].Style.Numberformat.Format = "yyyy-mm-dd";
                worksheet.Cells[row, 5].Value = customer.PhoneNumber;
                worksheet.Cells[row, 6].Value = customer.TotalOrders;
                row++;
            }

            // Align all data cells
            for (int i = 1; i <= customers.Count + 9; i++)
            {
                for (int j = 1; j <= headers.Length + 6; j++)
                {
                    worksheet.Cells[i, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                }
            }

            // Set column widths
            worksheet.Column(1).Width = 15;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 35;
            worksheet.Column(4).Width = 20;
            worksheet.Column(5).Width = 20;
            worksheet.Column(6).Width = 15;

            return package.GetAsByteArray();
        }
    }
}