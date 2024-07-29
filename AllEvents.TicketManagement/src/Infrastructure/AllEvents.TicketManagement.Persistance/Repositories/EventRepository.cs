using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.EntityFrameworkCore;

public class EventRepository : IEventRepository
{
    private readonly AllEventsDbContext _context;

    public EventRepository(AllEventsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetPagedEventsAsync(int pageIndex, int pageSize, bool includeDeleted = false)
    {
        IQueryable<Event> query = _context.Events;

        if (!includeDeleted)
        {
            query = query.Where(e => !e.IsDeleted);
        }

        return await query
            .OrderBy(e => e.EventDate)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Events.CountAsync(e => !e.IsDeleted);
    }

    public async Task<bool> ExistsAsync(Guid eventId)
    {
        return await _context.Events.AnyAsync(e => e.EventId == eventId);
    }

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        return await _context.Events.FindAsync(id);
    }

}