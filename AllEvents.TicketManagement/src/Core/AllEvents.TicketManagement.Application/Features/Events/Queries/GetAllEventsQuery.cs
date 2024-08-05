﻿using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Queries
{
    public class GetAllEventsQuery : IRequest<PagedResult<EventModel>>
    {
        public int Page { get; }
        public int PageSize { get; }
        public string? Title { get; }
        public EventCategory? Category { get; }
        public string? SortBy { get; }
        public bool Ascending { get; }

        public GetAllEventsQuery(int page, int pageSize, string? title, EventCategory? category, string? sortBy, bool ascending)
        {
            Page = page;
            PageSize = pageSize;
            Title = title;
            Category = category;
            SortBy = sortBy;
            Ascending = ascending;
        }
    }
}
