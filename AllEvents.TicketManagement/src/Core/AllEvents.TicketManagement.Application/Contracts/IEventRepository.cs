using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IEventRepository
    {
        Task<List<Event>> GetFilteredPagedEventsAsync(
            int pageIndex,
            int pageSize,
            string? title,
            EventCategory? category,
            string? sortBy,
            bool ascending);
        Task<int> GetCountAsync();
        Task<bool> ExistsAsync(Guid eventId);
        Task<int> GetFilteredCountAsync(string? title, EventCategory? category);
        Task<Event?> GetByIdAsync(Guid eventId);
    }
}
