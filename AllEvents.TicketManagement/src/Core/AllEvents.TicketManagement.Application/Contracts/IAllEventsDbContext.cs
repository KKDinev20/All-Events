using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IAllEventsDbContext
    {
        DbSet<Event> Events { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
