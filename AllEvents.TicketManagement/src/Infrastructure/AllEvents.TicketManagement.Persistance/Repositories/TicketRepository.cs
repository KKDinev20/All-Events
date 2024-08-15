using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistence.Repositories;

namespace AllEvents.TicketManagement.Persistance.Repositories
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(AllEventsDbContext dbContext) : base(dbContext)
        {
        }
    }
}
