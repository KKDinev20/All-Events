using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Orders.Commands;
using AllEvents.TicketManagement.Application.Features.ExternalUsers.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Application.Features.Orders.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, PlaceOrderResponse>
    {
        private readonly IAllEventsDbContext _context;
        private readonly IMediator _mediator;

        public CreateOrderCommandHandler(IAllEventsDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<PlaceOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.TicketNames.Count > 8)
            {
                throw new InvalidOperationException("Cannot order more than 8 tickets per event.");
            }

            var eventEntity = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == request.EventId, cancellationToken);

            if (eventEntity == null || eventEntity.NrOfTickets < request.TicketNames.Count)
            {
                throw new InvalidOperationException("Not enough tickets available or event does not exist.");
            }

            var createExternalUserCommand = new CreateExternalUserCommand(request.ExternalUserEmail);
            var externalUser = await _mediator.Send(createExternalUserCommand, cancellationToken);

            var existingOrders = await _context.Orders
                .Where(o => o.ExternalUserId == externalUser.Id && o.EventId == request.EventId)
                .ToListAsync(cancellationToken);

            var totalTicketsPurchased = existingOrders
                .SelectMany(o => o.TicketNames)
                .Count();

            if (totalTicketsPurchased + request.TicketNames.Count > 8)
            {
                var ticketsLeft = 8 - totalTicketsPurchased;
                throw new InvalidOperationException($"Only {ticketsLeft} tickets left for this event.");
            }

            eventEntity.NrOfTickets -= request.TicketNames.Count;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                Status = OrderStatus.Created,
                ExternalUserId = externalUser.Id,
                EventId = request.EventId,
                TicketNames = request.TicketNames
            };

            _context.Orders.Add(order);
            externalUser.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            var totalAmount = eventEntity.Price * request.TicketNames.Count;

            return new PlaceOrderResponse
            {
                OrderId = order.Id,
                TotalAmount = totalAmount
            };
        }
    }
}
