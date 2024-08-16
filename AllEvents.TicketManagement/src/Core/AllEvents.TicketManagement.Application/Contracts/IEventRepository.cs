using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<List<Event>> GetPagedEventsAsync(int page, int pageSize);
        Task<int> GetCountAsync();
        Task<bool> ExistsAsync(Guid eventId);
    }
}
