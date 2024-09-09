using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Orders.Commands
{
    public class PayOrderCommand : IRequest
    {
        public Guid OrderId { get; set; }
        public string? PromoCode { get; set; } 

        public PayOrderCommand(Guid orderId, string? promoCode = null)
        {
            OrderId = orderId;
            PromoCode = promoCode;
        }
    }
}
