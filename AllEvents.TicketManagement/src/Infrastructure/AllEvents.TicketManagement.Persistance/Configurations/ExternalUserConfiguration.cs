using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllEvents.TicketManagement.Persistance.Configurations
{
    public class ExternalUserConfiguration : IEntityTypeConfiguration<ExternalUser>
    {
        public void Configure(EntityTypeBuilder<ExternalUser> builder)
        {
            builder.HasKey(eu => eu.Id);

            builder.Property(eu => eu.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(eu => eu.Email)
                .IsUnique();

            builder.HasMany(eu => eu.Orders)
                .WithOne(o => o.ExternalUser)
                .HasForeignKey(o => o.ExternalUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
