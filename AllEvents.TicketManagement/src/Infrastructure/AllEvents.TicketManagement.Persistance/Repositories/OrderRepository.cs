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

        public async Task<List<Order>> GetOrdersByUserAndDateRangeAsync(Guid userId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Set<Order>()
                .Include(o => o.Event)
                .Where(o => o.ExternalUserId == userId)
                .AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedOn >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.CreatedOn <= toDate.Value);
            }

            return await query.OrderByDescending(o => o.CreatedOn).ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetTicketCountsByEventIdsAsync(List<Guid> eventIds)
        {
            return await _context.Set<Order>()
                .Where(o => eventIds.Contains(o.EventId) && o.Status == OrderStatus.Processing)
                .GroupBy(o => o.EventId)
                .Select(g => new
                {
                    EventId = g.Key,
                    TicketCount = g.Sum(o => o.TicketNames.Count)
                })
                .ToDictionaryAsync(x => x.EventId, x => x.TicketCount);
        }
    }
}
