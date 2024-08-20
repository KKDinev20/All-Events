using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.ExternalUsers.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Application.Features.ExternalUsers.Handlers
{
    public class CreateExternalUserCommandHandler : IRequestHandler<CreateExternalUserCommand, ExternalUser>
    {
        private readonly IAllEventsDbContext _context;

        public CreateExternalUserCommandHandler(IAllEventsDbContext context)
        {
            _context = context;
        }

        public async Task<ExternalUser> Handle(CreateExternalUserCommand request, CancellationToken cancellationToken)
        {
            var externalUser = await _context.ExternalUsers
                .FirstOrDefaultAsync(e => e.Email == request.ExternalUserEmail, cancellationToken);

            if (externalUser == null)
            {
                externalUser = new ExternalUser
                {
                    Id = Guid.NewGuid(),
                    Email = request.ExternalUserEmail
                };

                _context.ExternalUsers.Add(externalUser);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return externalUser;
        }
    }
}
