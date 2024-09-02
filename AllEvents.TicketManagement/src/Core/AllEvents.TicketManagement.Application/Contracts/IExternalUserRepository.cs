using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IExternalUserRepository : IRepository<ExternalUser>
    {
        Task<ExternalUser?> GetUserByEmailAsync(string email);
    }
}
