namespace AllEvents.TicketManagement.Domain.Entities
{
    public class PlaceOrderResponse
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
