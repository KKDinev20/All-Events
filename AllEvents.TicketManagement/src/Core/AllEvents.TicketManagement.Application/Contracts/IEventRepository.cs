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
        Task SoftDeleteAsync(Guid eventId);
        Task RestoreAsync(Guid eventId);
        Task<Event?> GetByIdAsync(Guid eventId);
    }
}
