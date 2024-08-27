using AllEvents.TicketManagement.Application.Models;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Tickets.Commands
{
    public class GenerateTicketCommand : IRequest<TicketModel>
    {
        public Guid EventId { get; set; }
        public Guid OrderId { get; set; }
        public string PersonName { get; set; } = null!;
    }
}
