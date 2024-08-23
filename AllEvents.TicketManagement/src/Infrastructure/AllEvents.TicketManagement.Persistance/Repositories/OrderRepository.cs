using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Persistence.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AllEventsDbContext context) : base(context)
        {
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Set<Order>().Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrdersByUserAsync(Guid externalUserId)
        {
            return await _context.Set<Order>()
                .Where(o => o.ExternalUserId == externalUserId)
                .ToListAsync();
        }
    }
}
