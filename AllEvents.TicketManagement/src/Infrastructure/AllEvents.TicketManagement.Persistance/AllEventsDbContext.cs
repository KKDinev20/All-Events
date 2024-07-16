using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Persistance
{
    public class AllEventsDbContext: IdentityDbContext
    {
        public AllEventsDbContext(DbContextOptions<AllEventsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConfiguration());

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.NrOfTickets).HasDefaultValue(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });
        }
    }
}
