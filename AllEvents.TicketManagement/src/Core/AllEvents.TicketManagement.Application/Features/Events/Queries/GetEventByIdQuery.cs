using AllEvents.TicketManagement.Domain.Entities;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Queries
{
    public class GetEventByIdQuery : IRequest<Event>
    {
        public Guid EventId { get; set; }
    }
}
