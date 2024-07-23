using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands.DeleteEvent;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Handlers
{
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, bool>
    {
        private readonly IAllEventsDbContext _context;

        public DeleteEventCommandHandler(IAllEventsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var @event = await _context.Events.FindAsync(request.EventId);

            if (@event == null || @event.IsDeleted)
            {
                return false;
            }

            @event.Delete();
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
