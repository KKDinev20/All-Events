using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                   .HasForeignKey(t => t.EventId);

        }
    }
}
