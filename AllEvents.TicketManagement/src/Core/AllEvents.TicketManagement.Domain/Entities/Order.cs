namespace AllEvents.TicketManagement.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public OrderStatus Status { get; set; }
        public Guid ExternalUserId { get; set; }
        public ExternalUser ExternalUser { get; set; } = null!;
        public Guid EventId { get; set; }
        public Event Event { get; set; } = null!;
        public List<string> TicketNames { get; set; } = new List<string>();
        public int TicketCount { get; set; }
    }

}
