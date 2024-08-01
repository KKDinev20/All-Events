﻿using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Persistance.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AllEventsDbContext _DbContext;

        public TicketRepository(AllEventsDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _DbContext.Tickets.AddAsync(ticket);
            await _DbContext.SaveChangesAsync();
        }

        public async Task<Ticket?> GetByIdAsync(Guid ticketID)
        {
            return await _DbContext.Tickets.FindAsync(ticketID);
        }
    }
}