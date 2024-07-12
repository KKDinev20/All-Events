using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllEvents.TicketManagement.Persistance.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.EventId);

            builder.Property(e => e.Title)
                   .IsRequired()
                   .HasMaxLength(2000); 

            builder.Property(e => e.Location)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(e => e.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(e => e.EventDate)
                   .IsRequired();

            builder.HasMany(e => e.Tickets)
                   .WithOne(t => t.Event)
                   .HasForeignKey(t => t.EventId);
        }
    }
}
