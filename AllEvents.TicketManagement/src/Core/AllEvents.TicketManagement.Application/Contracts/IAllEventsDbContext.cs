using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IAllEventsDbContext
    {
        DbSet<Event> Events { get; }
        DbSet<Order> Orders { get; }
        DbSet<ExternalUser> ExternalUsers { get; }
        DbSet<Coupon> Coupons { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
