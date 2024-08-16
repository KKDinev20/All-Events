namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
    }
}
