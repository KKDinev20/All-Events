namespace AllEvents.TicketManagement.Domain.Entities
{
    public class Coupon
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public int? DiscountPercent { get; set; } 
        public decimal? FixedDiscountAmount { get; set; } 
        public DateTime FromDate { get; set; } 
        public DateTime ToDate { get; set; } 
        public Guid? EventId { get; set; } 
        public Event? Event { get; set; }
    }
}
