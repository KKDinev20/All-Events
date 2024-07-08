using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Domain.Entities
{
    public class Event
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Price { get; set; }
        public EventCategory Category { get; set; }
    }
}
