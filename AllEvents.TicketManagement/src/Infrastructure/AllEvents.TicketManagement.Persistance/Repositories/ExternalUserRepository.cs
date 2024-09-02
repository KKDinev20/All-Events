using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Persistence.Repositories
{
    public class ExternalUserRepository : Repository<ExternalUser>, IExternalUserRepository
    {
        public ExternalUserRepository(AllEventsDbContext context) : base(context)
        {
        }

        public async Task<ExternalUser?> GetUserByEmailAsync(string email)
        {
            return await _context.Set<ExternalUser>()
                                 .Include(u => u.Orders)
                                 .ThenInclude(o => o.Event)
                                 .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
