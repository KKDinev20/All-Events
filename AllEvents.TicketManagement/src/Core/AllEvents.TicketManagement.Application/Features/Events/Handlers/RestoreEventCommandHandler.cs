using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Handlers
{
    public class RestoreEventCommandHandler : IRequestHandler<RestoreEventCommand, bool>
    {
        private readonly IAllEventsDbContext _context;

        public RestoreEventCommandHandler(IAllEventsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RestoreEventCommand request, CancellationToken cancellationToken)
        {
            var @event = await _context.Events.FindAsync(request.EventId);

            if (@event == null || !@event.IsDeleted)
            {
                return false;
            }

            @event.Restore();
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
