using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Application.Features.Events.Handlers
{
    public class RestoreEventCommandHandler: IRequestHandler<RestoreEventCommand>
    {
        private readonly IEventRepository _eventRepository;

        public RestoreEventCommandHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Unit> Handle(RestoreEventCommand request, CancellationToken cancellationToken)
        {
            await _eventRepository.RestoreAsync(request.EventId);
            return Unit.Value;
        }
    }
}
