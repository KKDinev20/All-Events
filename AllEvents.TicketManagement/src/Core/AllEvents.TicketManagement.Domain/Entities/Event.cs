namespace AllEvents.TicketManagement.Domain.Entities
{
    public class Event
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Price { get; set; }
        public EventCategory Category { get; set; }

        public DateTime EventDate { get; set; }

        public int NrOfTickets { get; set; } = 100;
        public bool IsDeleted { get; set; } = false;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public void Delete()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }
    }
}
