using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Customerorder> GetAll()
        {
            return _context.Customerorders.AsQueryable();
        }

        public async Task<Customerorder> GetOrderWithDetails(int orderId)
        {
            return await _context.Customerorders
                .Include(o => o.Customer)
                .Include(o => o.Ordertables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t.Section)
                .Include(o => o.Orderdetails)
                    .ThenInclude(od => od.Item)
                .Include(o => o.Orderdetails)
                    .ThenInclude(od => od.Orderdetailmodifiers)
                        .ThenInclude(odm => odm.Modifier)
                .Include(o => o.Ordertaxes)
                    .ThenInclude(ot => ot.Tax)
                .Include(o => o.Invoices)
                .FirstOrDefaultAsync(o => o.Orderid == orderId);
        }
    }
}