using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Application.Features.Events.Commands
{
    public class RestoreEventCommand: IRequest
    {
        public Guid EventId { get; set; }

        public RestoreEventCommand(Guid eventId)
        {
            EventId = eventId;
        }
    }
}
