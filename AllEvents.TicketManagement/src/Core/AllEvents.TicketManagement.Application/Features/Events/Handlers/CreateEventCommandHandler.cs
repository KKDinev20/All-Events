using MediatR;
using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Application.Features.Events.Commands
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand>
    {
        private readonly IAllEventsDbContext _context;

        public CreateEventCommandHandler(IAllEventsDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {

            var newEvent = new Event
            {
                EventId = Guid.NewGuid(),
                Title = request.Title,
                Location = request.Location,
                Price = request.Price,
                Category = request.Category,
                EventDate = request.EventDate,
                NrOfTickets = request.NrOfTickets,
                IsDeleted = false 
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
