using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Commands.DeleteEvent
{
    public class DeleteEventCommand : IRequest
    {
        public Guid EventId { get; set; }

        public DeleteEventCommand(Guid eventId)
        {
            EventId = eventId;
        }
    }
}
