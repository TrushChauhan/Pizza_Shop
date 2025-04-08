using Entity.Models;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<Customerorder>> GetOrders(OrderFilterModel filters)
    {
        var query = _orderRepository.GetAll()
            .Include(o => o.Customer)
            .Where(o => !o.Isdeleted);

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
            query = query.Where(o => o.Date >= DateOnly.FromDateTime(filters.FromDate.Value));
        }

        if (filters.ToDate.HasValue)
        {
            query = query.Where(o => o.Date <= DateOnly.FromDateTime(filters.ToDate.Value));
        }

        if (filters.TimePeriod != "All time" && !string.IsNullOrEmpty(filters.TimePeriod))
        {
            var now = DateTime.Now;
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

        // Apply sorting
        if (!string.IsNullOrEmpty(filters.SortField))
        {
            switch (filters.SortField)
            {
                case "Order":
                    query = filters.SortAscending ?
                        query.OrderBy(o => o.Orderid) :
                        query.OrderByDescending(o => o.Orderid);
                    break;
                case "Date":
                    query = filters.SortAscending ?
                        query.OrderBy(o => o.Date) :
                        query.OrderByDescending(o => o.Date);
                    break;
                case "Customer":
                    query = filters.SortAscending ?
                        query.OrderBy(o => o.Customer.Customername) :
                        query.OrderByDescending(o => o.Customer.Customername);
                    break;
                case "Total Amount":
                    query = filters.SortAscending ?
                        query.OrderBy(o => o.Totalamount) :
                        query.OrderByDescending(o => o.Totalamount);
                    break;
            }
        }
        if (filters.PageSize > 0)
        {
            query = query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize);
        }

        return await query.ToListAsync();
    }
}
