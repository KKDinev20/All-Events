using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Tickets.Commands
{
    public class ValidateTicketCommand : IRequest<string>
    {
        public string Token { get; }

        public ValidateTicketCommand(string token)
        {
            Token = token;
        }
    }
}
