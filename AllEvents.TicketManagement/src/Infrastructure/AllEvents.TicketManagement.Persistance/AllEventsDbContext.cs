using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Persistance
{
    public class AllEventsDbContext: DbContext
    {
        public AllEventsDbContext(DbContextOptions<AllEventsDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }

    }
}
