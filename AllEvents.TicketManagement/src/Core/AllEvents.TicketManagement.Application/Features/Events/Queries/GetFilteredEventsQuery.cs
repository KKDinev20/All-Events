using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Queries
{
    public class GetFilteredEventsQuery : IRequest<PagedResult<EventModel>>
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public string? Title { get; }
        public EventCategory? Category { get; }
        public string? SortBy { get; }
        public bool Ascending { get; }

        public GetFilteredEventsQuery(int pageIndex, int pageSize, string? title, EventCategory? category, string? sortBy, bool ascending)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Title = title;
            Category = category;
            SortBy = sortBy;
            Ascending = ascending;
        }
    }
}
