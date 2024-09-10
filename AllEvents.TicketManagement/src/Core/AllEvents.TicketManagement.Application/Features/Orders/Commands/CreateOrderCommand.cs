using AllEvents.TicketManagement.Domain.Entities;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<PlaceOrderResponse>
    {
        public string ExternalUserEmail { get; set; } = null!;
        public Guid EventId { get; set; }
        public List<string> TicketNames { get; set; } = new List<string>();
        public string? PromoCode { get; set; }
    }
}
