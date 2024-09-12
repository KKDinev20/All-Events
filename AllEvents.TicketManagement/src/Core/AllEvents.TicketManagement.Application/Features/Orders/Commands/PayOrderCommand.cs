using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Orders.Commands
{
    public class PayOrderCommand : IRequest
    {
        public Guid OrderId { get; set; }

        public PayOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
