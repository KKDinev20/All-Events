using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<Event>> GetPagedEventsAsync(int page, int pageSize)
        {
            return await _context.Events
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
