namespace AllEvents.TicketManagement.Domain.Entities
{
    public class ExternalUser
    {
        public Guid Id { get; set; }  
        public string Email { get; set; } = null!;
        public decimal TotalAmountSpent { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>(); 
    }
}