using AllEvents.TicketManagement.Application.Features.Events.Commands;
using AllEvents.TicketManagement.Application.Features.Events.Commands.DeleteEvent;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllEvents.TicketManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<EventModel>>> RetrieveAllEvents(int page = 1, int pageSize = 10)
        {
            var query = new GetAllEventsQuery(page, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var result = await _mediator.Send(new DeleteEventCommand { EventId = id });
            if (result)
            {
                return Ok(new { Message = "Event deleted successfully." });
            }

            return NotFound(new { Message = "Event not found or already deleted." });
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreEvent(Guid id)
        {
            var result = await _mediator.Send(new RestoreEventCommand { EventId = id });
            if (result)
            {
                return Ok(new { Message = "Event restored successfully." });
            }

            return NotFound(new { Message = "Event not found or not deleted." });
        }
    }
}
