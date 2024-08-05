using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
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

    public IEventQuery GetEvents()
    {
        return new EventQuery(_context.Events.AsQueryable());
    }

    public async Task<List<Event>> GetPagedEventsAsync(int page, int pageSize)
    {
        if (page < 0 || pageSize <= 0)
        {
            return new List<Event>();
        }

        return await _context.Events
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Event>> GetFilteredPagedEventsAsync(
        int pageIndex,
        int pageSize,
        string? title,
        EventCategory? category,
        string? sortBy,
        bool ascending)
    {
        var query = GetEvents();

        if (!string.IsNullOrEmpty(title))
        {
            query = query.Search(title);
        }

        if (category.HasValue)
        {
            query = query.ForCategory(category.Value);
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = query.SortBy(sortBy, ascending);
        }

        return await query.ToListAsync(pageIndex, pageSize);
    }

    public async Task<int> GetFilteredCountAsync(string? title, EventCategory? category)
    {
        var query = GetEvents();

        if (!string.IsNullOrEmpty(title))
        {
            query = query.Search(title);
        }

        if (category.HasValue)
        {
            query = query.ForCategory(category.Value);
        }

        return await query.CountAsync();
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
