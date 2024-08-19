using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllEvents.TicketManagement.Persistance.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t => t.TicketId);

            builder.Property(t => t.PersonName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.EventTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.QRCode)
                .IsRequired();

            builder.HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne<Order>() 
                .WithMany() 
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
