using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Orders.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            .Include(o => o.Event)
            .Include(o => o.ExternalUser)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            throw new InvalidOperationException("Order does not exist.");
        }

        if (order.Status != OrderStatus.Created)
        {
            throw new InvalidOperationException("Only orders in 'Created' state can be processed.");
        }

        order.Status = OrderStatus.Processing;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
