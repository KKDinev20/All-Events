using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IEventRepository
    {
        Task<int> GetCountAsync();
        Task<List<Event>> GetPagedEventsAsync(int page, int pageSize);
        Task<Event> GetByIdAsync(Guid eventId);
    }
}
