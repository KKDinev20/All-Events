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
<<<<<<< HEAD
        Task<int> GetFilteredCountAsync(string? title, EventCategory? category);
=======
>>>>>>> ac87d130287dac07e60038171fef98071293055b
        Task<Event?> GetByIdAsync(Guid eventId);
    }
}
