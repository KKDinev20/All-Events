using AllEvents.TicketManagement.Application.Models;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Tickets.Commands
{
    public class ValidateTicketCommand : IRequest<ValidationResult>
    {
        public string Token { get; set; }
    }
}
