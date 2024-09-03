using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Persistance.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(AllEventsDbContext context) : base(context)
        {
        }

        public async Task<List<Event>> GetPagedEventsAsync(int page, int pageSize)
        {
            if (page < 0 || pageSize <= 0)
            {
                return new List<Event>();
            }

            return await _context.Set<Event>()
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Set<Event>().CountAsync(e => !e.IsDeleted);
        }

        public async Task<bool> ExistsAsync(Guid eventId)
        {
            return await _context.Set<Event>().AnyAsync(e => e.EventId == eventId);
        }

        public async Task<List<Event>> GetEventsByCategoryAndDateRangeAsync(EventCategory category, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Set<Event>()
                                .Where(e => e.Category == category && !e.IsDeleted);

            if (fromDate.HasValue)
            {
                query = query.Where(e => e.EventDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(e => e.EventDate <= toDate.Value);
            }

            return await query.ToListAsync();
        }
    }
}
