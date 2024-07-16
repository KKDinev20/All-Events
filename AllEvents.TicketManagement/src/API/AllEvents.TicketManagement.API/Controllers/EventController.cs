using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IEventRepository _eventRepository;

    public EventController(IMediator mediator, IEventRepository eventRepository)
    {
        _mediator = mediator;
        _eventRepository = eventRepository;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<EventModel>>> RetrieveAllEvents(int page = 1, int pageSize = 10)
    {
        var query = new GetAllEventsQuery(page, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteEvent(Guid id)
    {
        var eventExists = await _eventRepository.ExistsAsync(id);
        if (!eventExists)
        {
            return NotFound(new { Message = "Event not found" });
        }

        await _eventRepository.SoftDeleteAsync(id);
        return Ok(new { Message = "Event successfully deleted" });
    }

    [HttpPut("restore/{id}")]
    public async Task<IActionResult> RestoreEvent(Guid id)
    {
        var eventExists = await _eventRepository.ExistsAsync(id);
        if (!eventExists)
        {
            return NotFound(new { Message = "Event not found" });
        }

        await _eventRepository.RestoreAsync(id);
        return Ok(new { Message = "Event successfully restored" });
    }

}
