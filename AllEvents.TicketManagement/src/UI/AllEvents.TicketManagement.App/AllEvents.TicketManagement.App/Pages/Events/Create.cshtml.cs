using AllEvents.TicketManagement.Application.Features.Events.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllEvents.TicketManagement.App.Pages.Events
{
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;

        public CreateModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public string Title { get; set; } = null!;
        [BindProperty]
        public string Location { get; set; } = null!;
        [BindProperty]
        public decimal Price { get; set; }
        [BindProperty]
        public EventCategory Category { get; set; }
        [BindProperty]
        public DateTime EventDate { get; set; }
        [BindProperty]
        public int NrOfTickets { get; set; } = 100;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var command = new CreateEventCommand
            {
                Title = Title,
                Location = Location,
                Price = Price,
                Category = Category,
                EventDate = EventDate,
                NrOfTickets = NrOfTickets
            };

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = "Event created successfully.";
                return RedirectToPage("/Events/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to create event: {ex.Message}");
                return Page();
            }
        }
    }
}
