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
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(ev => ev.EventId);
            builder.Property(ev => ev.Title)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(ev => ev.Location)
                .IsRequired()
                .HasMaxLength(80);
            builder.Property(ev => ev.Price)
                .IsRequired();
            builder.Property(ev => ev.Category)
                .IsRequired();
        }
    }
}
