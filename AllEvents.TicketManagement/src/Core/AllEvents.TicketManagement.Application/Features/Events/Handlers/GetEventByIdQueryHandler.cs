using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Application.Features.Events.Queries
{
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, Event>
    {
        private readonly IAllEventsDbContext _dbContext;

        public GetEventByIdQueryHandler(IAllEventsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Event> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Events
                .FirstOrDefaultAsync(e => e.EventId == request.EventId, cancellationToken);
        }
    }
}
