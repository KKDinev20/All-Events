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

            builder.HasOne(o => o.ExternalUser)
                .WithMany(eu => eu.Orders)
                .HasForeignKey(o => o.ExternalUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.TicketNames)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        }
    }
}
