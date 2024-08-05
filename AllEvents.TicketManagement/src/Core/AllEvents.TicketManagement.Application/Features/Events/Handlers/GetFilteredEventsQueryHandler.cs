using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Application.Models;
using MediatR;

public class GetFilteredEventsQueryHandler : IRequestHandler<GetFilteredEventsQuery, PagedResult<EventModel>>
{
    private readonly IAllEventsDbContext _dbContext;

    public GetFilteredEventsQueryHandler(IAllEventsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<EventModel>> Handle(GetFilteredEventsQuery request, CancellationToken cancellationToken)
    {
        var query = new EventQuery(_dbContext.Events);

        if (!string.IsNullOrEmpty(request.Title))
        {
            query.Search(request.Title);
        }

        if (request.Category.HasValue)
        {
            query.ForCategory(request.Category.Value);
        }

        if (!string.IsNullOrEmpty(request.SortBy))
        {
            query.SortBy(request.SortBy, request.Ascending);
        }

        var events = await query.ToListAsync(request.PageIndex, request.PageSize);
        var totalEvents = await query.CountAsync();

        var eventModels = events.Select(e => new EventModel
        {
            EventId = e.EventId,
            Title = e.Title,
            Location = e.Location,
            Price = e.Price,
            Category = e.Category,
        }).ToList();

        return new PagedResult<EventModel>(eventModels, totalEvents, request.PageIndex + 1, request.PageSize);
    }
}
