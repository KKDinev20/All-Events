using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.App.Model
{
    public class EventCreateDto
    {
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Price { get; set; }
        public EventCategory Category { get; set; }
        public DateTime EventDate { get; set; }
        public int NrOfTickets { get; set; } = 100;
    }
}
