using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IEventQuery
    {
        IEventQuery Search(string query);
        IEventQuery ForCategory(EventCategory category);
        IEventQuery SortBy(string sortBy, bool ascending);
        Task<List<Event>> ToListAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
    }

}
