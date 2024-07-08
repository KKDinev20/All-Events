using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Persistance.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AllEventsDbContext _context;

        public EventRepository(AllEventsDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Events.CountAsync();
        }

        public async Task<List<EventModel>> GetPagedEventsAsync(int page, int pageSize)
        {
            var events = await _context.Events
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return events.Select(e => new EventModel
            {
                EventId = e.EventId,
                Title = e.Title,
                Location = e.Location,
                Price = e.Price,
                Category = e.Category
            }).ToList();
        }
    }
}
