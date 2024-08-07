using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Extensions;
using AllEvents.TicketManagement.Application.Features.Events.Commands.DeleteEvent;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, bool>
{
    private readonly IAllEventsDbContext _context;
    private readonly IDistributedCache _cache;
    private const string CacheKeyPrefix = "Event";

    public DeleteEventCommandHandler(IAllEventsDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
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

        await _cache.InvalidateCacheAsync(CacheKeyPrefix);

        return true;
    }
}
