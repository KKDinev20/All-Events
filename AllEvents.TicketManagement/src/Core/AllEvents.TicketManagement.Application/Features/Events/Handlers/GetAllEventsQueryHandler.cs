using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Handlers
{
    public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, PagedResult<Event>>
    {
        private readonly IEventRepository eventRepository;

        public GetAllEventsQueryHandler(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }

        public async Task<PagedResult<Event>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
        {
            var totalCount = await eventRepository.GetCountAsync();

            var items = await eventRepository.GetPagedEventsAsync(request.Page, request.PageSize);

            return new PagedResult<Event>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
