using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IEventRepository
    {
        Task<List<Event>> GetPagedEventsAsync(int pageIndex, int pageSize, bool includeDeleted = false);
        Task<int> GetCountAsync();
        Task<bool> ExistsAsync(Guid eventId);
        Task SoftDeleteAsync(Guid eventId);
        Task RestoreAsync(Guid eventId);
        Task<Event?> GetByIdAsync(Guid eventId);
    }
}
