using AllEvents.TicketManagement.Domain.Entities;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.ExternalUsers.Commands
{
    public class CreateExternalUserCommand : IRequest<ExternalUser>
    {
        public string ExternalUserEmail { get; set; }

        public CreateExternalUserCommand(string externalUserEmail)
        {
            ExternalUserEmail = externalUserEmail;
        }
    }

}
