using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task UpdateAsync(Order order);
        Task<List<Order>> GetOrdersByUserAsync(Guid externalUserId);
        Task<List<Order>> GetOrdersByEventIdsAsync(List<Guid> eventIds);
        Task<List<Order>> GetOrdersByUserAndDateRangeAsync(Guid userId, DateTime? fromDate, DateTime? toDate);
    }
}
