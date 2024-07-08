using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Queries
{
    public class GetAllEventsQuery: IRequest<PagedResult<EventModel>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public GetAllEventsQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
