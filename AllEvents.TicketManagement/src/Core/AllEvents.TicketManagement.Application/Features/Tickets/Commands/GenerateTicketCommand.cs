using AllEvents.TicketManagement.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Application.Features.Tickets.Commands
{
    public class GenerateTicketCommand : IRequest<TicketModel>
    {
        public Guid EventId { get; set; }
        public string PersonName { get; set; }
    }
}
