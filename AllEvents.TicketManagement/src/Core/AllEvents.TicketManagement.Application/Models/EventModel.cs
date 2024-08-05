using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Models
{
    public class EventModel
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Price { get; set; }
        public EventCategory Category { get; set; }

        public DateTime EventDate { get; set; }

    }
}
