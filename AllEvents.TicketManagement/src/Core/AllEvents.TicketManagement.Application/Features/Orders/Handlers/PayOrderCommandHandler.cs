using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Orders.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Application.Features.Orders.Handlers
{
    public class PayOrderCommandHandler : IRequestHandler<PayOrderCommand>
    {
        private readonly IAllEventsDbContext _context;

        public PayOrderCommandHandler(IAllEventsDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(PayOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
            {
                throw new InvalidOperationException("Order does not exist.");
            }

            if (order.Status != OrderStatus.Created)
            {
                throw new InvalidOperationException("Only orders in 'Created' state can be processed.");
            }

            Coupon? coupon = null;
            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Name == request.PromoCode &&
                                              c.FromDate <= DateTime.Now &&
                                              c.ToDate >= DateTime.Now, cancellationToken);

                if (coupon == null)
                {
                    throw new InvalidOperationException("Invalid or expired promo code.");
                }
            }

            if (coupon != null)
            {
                decimal discountAmount = 0;

                if (coupon.DiscountPercent.HasValue)
                {
                    discountAmount = order.TotalPrice * (coupon.DiscountPercent.Value / 100m);
                }
                else if (coupon.FixedDiscountAmount.HasValue)
                {
                    discountAmount = coupon.FixedDiscountAmount.Value;
                }

                order.TotalPrice = Math.Max(order.TotalPrice - discountAmount, 0);
            }

            order.Status = OrderStatus.Processing;

            _context.Orders.Update(order);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
