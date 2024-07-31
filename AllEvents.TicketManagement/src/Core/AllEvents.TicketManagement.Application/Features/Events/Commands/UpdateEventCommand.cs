using AllEvents.TicketManagement.Domain.Entities;
using MediatR;

namespace AllEvents.TicketManagement.Application.Features.Events.Commands
{
    public class UpdateEventCommand : IRequest
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Price { get; set; }
        public EventCategory Category { get; set; }
        public DateTime EventDate { get; set; }
        public int NrOfTickets { get; set; } = 100;
    }
}
