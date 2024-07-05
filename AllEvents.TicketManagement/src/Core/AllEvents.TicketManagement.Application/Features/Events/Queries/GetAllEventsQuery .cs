using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Application.Features.Events.Queries
{
    public class GetAllEventsQuery: IRequest<PagedResult<Event>>
    {
        public int Page = 1;
        public int PageSize = 10;
    }
}
