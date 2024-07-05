using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
namespace AllEvents.TicketManagement.Application.Features.Events.Handlers
{
    public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, PagedResult<Event>>
    {

        public Task<PagedResult<Event>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
