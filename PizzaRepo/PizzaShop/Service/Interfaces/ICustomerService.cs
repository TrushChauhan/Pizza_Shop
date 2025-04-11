using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces
{
    public interface ICustomerService
    {
        Task<(List<CustomerViewModel> Customers, int TotalCount)> GetCustomers(CustomerFilterModel filters);
        Task<byte[]> ExportCustomersToExcelAsync(CustomerFilterModel filters);
    }
}