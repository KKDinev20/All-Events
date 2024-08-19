using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance.Caching;
using AllEvents.TicketManagement.Persistance.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AllEvents.TicketManagement.Persistance
{
    public class AllEventsDbContext : IdentityDbContext, IAllEventsDbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public AllEventsDbContext(DbContextOptions<AllEventsDbContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ExternalUser> ExternalUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new ExternalUserConfiguration());

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.NrOfTickets).HasDefaultValue(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Email)
                .WithMany(eu => eu.Orders)
                .HasForeignKey(o => o.ExternalUserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Ticket>()
                .HasOne<Order>()
                .WithMany() 
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLoggerFactory(_loggerFactory)
                    .AddInterceptors(new QueryHandlerInterceptor(_loggerFactory.CreateLogger<QueryHandlerInterceptor>(), TimeSpan.FromSeconds(2))); // Set threshold to 2 seconds
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
