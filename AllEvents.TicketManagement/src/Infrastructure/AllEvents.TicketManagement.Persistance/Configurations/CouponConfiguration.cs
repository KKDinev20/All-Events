using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Persistance.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.DiscountPercent);
            builder.Property(c => c.FixedDiscountAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.FromDate).IsRequired();
            builder.Property(c => c.ToDate).IsRequired();

            builder.HasOne(c => c.Event)
                   .WithMany(e => e.Coupons)
                   .HasForeignKey(c => c.EventId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasCheckConstraint("CK_Coupon_Discount",
                "(DiscountPercent IS NOT NULL AND FixedDiscountAmount IS NULL) OR " +
                "(DiscountPercent IS NULL AND FixedDiscountAmount IS NOT NULL)");
        }
    }
}
