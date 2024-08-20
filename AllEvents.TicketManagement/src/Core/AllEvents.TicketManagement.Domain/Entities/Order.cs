namespace AllEvents.TicketManagement.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public OrderStatus Status { get; set; }
        public Guid ExternalUserId { get; set; }  
        public ExternalUser Email { get; set; } = null!;
        public Guid EventId { get; set; }
        public ICollection<string> TicketNames { get; set; } = new List<string>();
    }
}
