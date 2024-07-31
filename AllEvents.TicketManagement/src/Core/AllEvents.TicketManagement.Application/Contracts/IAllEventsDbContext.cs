using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IAllEventsDbContext
    {
        DbSet<Event> Events { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
