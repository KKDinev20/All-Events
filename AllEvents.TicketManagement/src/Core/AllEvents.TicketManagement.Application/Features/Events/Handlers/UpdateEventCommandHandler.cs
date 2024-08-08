using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Extensions;
using AllEvents.TicketManagement.Application.Features.Events.Commands;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand>
{
    private readonly IAllEventsDbContext _dbContext;
    private readonly IDistributedCache _cache;
    private readonly IValidator<UpdateEventCommand> _validator;
    private const string CacheKeyPrefix = "Event";

    public UpdateEventCommandHandler(IAllEventsDbContext dbContext, IDistributedCache cache, IValidator<UpdateEventCommand> validator)
    {
        _dbContext = dbContext;
        _cache = cache;
        _validator = validator;
    }

    public async Task<Unit> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var @event = await _dbContext.Events
            .FirstOrDefaultAsync(e => e.EventId == request.EventId, cancellationToken);

        if (@event == null)
        {
            throw new KeyNotFoundException($"Event with ID {request.EventId} not found.");
        }

        @event.Title = request.Title;
        @event.Location = request.Location;
        @event.Price = request.Price;
        @event.Category = request.Category;
        @event.EventDate = request.EventDate;
        @event.NrOfTickets = request.NrOfTickets;

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _cache.InvalidateCacheAsync(CacheKeyPrefix);

        return Unit.Value;
    }
}
