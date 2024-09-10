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
                .Sum(o => o.TicketCount);

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
                TicketNames = request.TicketNames,
                TicketCount = request.TicketNames.Count
            };

            _context.Orders.Add(order);
            externalUser.Orders.Add(order);

            decimal totalAmount = await ApplyCouponDiscount(request, eventEntity, cancellationToken);

            var totalAmountSpent = await _context.Orders
                .Where(o => o.ExternalUserId == externalUser.Id)
                .SumAsync(o => o.TicketCount * eventEntity.Price, cancellationToken);

            decimal loyaltyDiscount = ApplyLoyaltyDiscount(totalAmountSpent);

            totalAmount -= totalAmount * loyaltyDiscount;
            totalAmount = Math.Max(totalAmount, 0);

            externalUser.TotalAmountSpent += totalAmount;  
            _context.ExternalUsers.Update(externalUser); 

            order.TotalPrice = totalAmount;

            await _context.SaveChangesAsync(cancellationToken);

            return new PlaceOrderResponse
            {
                OrderId = order.Id,
                TotalAmount = totalAmount
            };
        }

        private async Task<decimal> ApplyCouponDiscount(CreateOrderCommand request, Event? eventEntity, CancellationToken cancellationToken)
        {
            decimal totalAmount = eventEntity.Price * request.TicketNames.Count;

            if (!string.IsNullOrEmpty(request.PromoCode))
            {
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Name == request.PromoCode && c.FromDate <= DateTime.UtcNow && c.ToDate >= DateTime.UtcNow, cancellationToken);

                if (coupon != null)
                {
                    if (coupon.DiscountPercent.HasValue)
                    {
                        totalAmount -= (totalAmount * coupon.DiscountPercent.Value / 100);
                    }
                    else if (coupon.FixedDiscountAmount.HasValue)
                    {
                        totalAmount -= coupon.FixedDiscountAmount.Value;
                    }
                }
            }

            return totalAmount;
        }

        private static decimal ApplyLoyaltyDiscount(decimal totalAmountSpent)
        {
            decimal loyaltyDiscount = 0;
            if (totalAmountSpent >= 1000)
            {
                loyaltyDiscount = 0.05m;
            }
            else if (totalAmountSpent >= 500)
            {
                loyaltyDiscount = 0.02m;
            }

            return loyaltyDiscount;
        }
    }
}
