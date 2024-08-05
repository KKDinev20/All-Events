using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Application.Models;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Handlers
{
    public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, PagedResult<EventModel>>
    {
        private readonly IEventQuery _eventQuery;

        public GetAllEventsQueryHandler(IEventQuery eventQuery)
        {
            _eventQuery = eventQuery;
        }

        public async Task<PagedResult<EventModel>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
        {
            var query = _eventQuery.ExcludeDeleted();

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

<<<<<<< HEAD
            var events = await query.ToListAsync(request.Page - 1, request.PageSize);
=======
            var events = await query.ToListAsync(request.Page - 1, request.PageSize); 

>>>>>>> e801c393e72e865b37ee0cf68ae4a76a691cd20f
            var totalCount = await query.CountAsync();

            var items = events.Select(e => new EventModel
            {
                EventId = e.EventId,
                Title = e.Title,
                Location = e.Location,
                Price = e.Price,
                Category = e.Category,
            }).ToList();

            return new PagedResult<EventModel>(items, totalCount, request.Page, request.PageSize);
        }
    }
}
