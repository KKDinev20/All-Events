using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllEvents.TicketManagement.Persistance.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.CreatedOn)
                .IsRequired();

            builder.Property(o => o.Status)
                .IsRequired();

            builder.Property(o => o.ExternalUserId)
                .IsRequired();

            builder.Property(o => o.EventId)
                .IsRequired();

            builder.HasIndex(o => new { o.ExternalUserId, o.EventId });
        }
    }
}
