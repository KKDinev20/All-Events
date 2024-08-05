using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IEventRepository
    {
        Task<int> GetCountAsync();
        Task<bool> ExistsAsync(Guid eventId);
        Task<Event?> GetByIdAsync(Guid eventId);
    }
}
