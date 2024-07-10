using AllEvents.TicketManagement.Application.Features.Tickets.Commands;
using AllEvents.TicketManagement.Application.Models;

namespace AllEvents.TicketManagement.Application.Features.Tickets.Handlers
{
    public interface IGenerateTicketCommandHandler
    {
        Task<TicketModel> Handle(GenerateTicketCommand request, CancellationToken cancellationToken);
    }
}