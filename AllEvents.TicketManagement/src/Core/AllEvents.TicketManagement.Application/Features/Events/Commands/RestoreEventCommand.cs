using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Commands
{
    public class RestoreEventCommand : IRequest<bool>
    {
        public Guid EventId { get; set; }
    }

}
