using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Commands.DeleteEvent
{
    public class DeleteEventCommand : IRequest<bool>
    {
        public Guid EventId { get; set; }
    }
}
