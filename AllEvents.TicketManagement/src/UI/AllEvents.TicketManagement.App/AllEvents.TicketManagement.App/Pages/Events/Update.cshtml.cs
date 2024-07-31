using AllEvents.TicketManagement.Application.Features.Events.Commands;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllEvents.TicketManagement.App.Pages.Events
{
    public class UpdateModel : PageModel
    {
        private readonly IMediator _mediator;

        public UpdateModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public Event Event { get; set; } = new Event();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var query = new GetEventByIdQuery { EventId = id };
            Event = await _mediator.Send(query);

            if (Event == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var command = new UpdateEventCommand
            {
                EventId = Event.EventId,
                Title = Event.Title,
                Location = Event.Location,
                Price = Event.Price,
                Category = Event.Category,
                EventDate = Event.EventDate,
                NrOfTickets = Event.NrOfTickets
            };

            await _mediator.Send(command);

            return RedirectToPage("/Events/Index");
        }
    }
}
