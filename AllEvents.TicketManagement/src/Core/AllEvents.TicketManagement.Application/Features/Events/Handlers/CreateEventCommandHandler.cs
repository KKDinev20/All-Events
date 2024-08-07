using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Extensions;
using AllEvents.TicketManagement.Application.Features.Events.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand>
{
    private readonly IAllEventsDbContext _context;
    private readonly IDistributedCache _cache;
    private const string CacheKeyPrefix = "Event";

    public CreateEventCommandHandler(IAllEventsDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Unit> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var newEvent = new Event
        {
            EventId = Guid.NewGuid(),
            Title = request.Title,
            Location = request.Location,
            Price = request.Price,
            Category = request.Category,
            EventDate = request.EventDate,
            NrOfTickets = request.NrOfTickets,
            IsDeleted = false
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.InvalidateCacheAsync(CacheKeyPrefix);

        return Unit.Value;
    }
}
