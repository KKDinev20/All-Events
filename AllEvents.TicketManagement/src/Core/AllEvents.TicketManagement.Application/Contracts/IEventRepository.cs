using AllEvents.TicketManagement.Application.Models;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IEventRepository
    {
        Task<int> GetCountAsync();
        Task<List<EventModel>> GetPagedEventsAsync(int page, int pageSize);
    }
}
