using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Application.Features.Events.Queries
{
    public class EventQuery: IEventQuery
    {
        private IQueryable<Event> _query;

        public EventQuery(IQueryable<Event> query)
        {
            _query = query;
        }

        public IEventQuery Search(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                _query = _query.Where(e => e.Title.Contains(query));
            }
            return this;
        }

        public IEventQuery ForCategory(EventCategory category)
        {
            _query = _query.Where(e => e.Category == category);
            return this;
        }

        public IEventQuery SortBy(string sortBy, bool ascending)
        {
            if (!string.IsNullOrEmpty(sortBy))
            {
                _query = ascending
                    ? _query.OrderBy(e => EF.Property<object>(e, sortBy))
                    : _query.OrderByDescending(e => EF.Property<object>(e, sortBy));
            }
            return this;
        }

        public async Task<List<Event>> ToListAsync(int pageIndex, int pageSize)
        {
            return await _query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _query.CountAsync();
        }
    }
}
