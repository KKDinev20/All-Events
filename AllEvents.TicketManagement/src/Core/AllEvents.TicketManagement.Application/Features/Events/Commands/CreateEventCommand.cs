using MediatR;
using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Features.Events.Commands
{
    public class CreateEventCommand : IRequest
    {
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Price { get; set; }
        public EventCategory Category { get; set; }
        public DateTime EventDate { get; set; }
        public int NrOfTickets { get; set; } = 100;
    }
}
