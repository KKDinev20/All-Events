using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Application.Models;
using MediatR;

public class GetFilteredEventsQueryHandler : IRequestHandler<GetFilteredEventsQuery, PagedResult<EventModel>>
{
    private readonly IEventRepository _eventRepository;

    public GetFilteredEventsQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<PagedResult<EventModel>> Handle(GetFilteredEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventRepository.GetFilteredPagedEventsAsync(
            request.PageIndex,
            request.PageSize,
            request.Title,
            request.Category,
            request.SortBy,
            request.Ascending);

        var totalEvents = await _eventRepository.GetCountAsync();

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
