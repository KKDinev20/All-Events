using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllEvents.TicketManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IMediator mediator;

        public EventController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<EventModel>>> RetrieveAllEvents(int page = 1, int pageSize = 10)
        {
            var query = new GetAllEventsQuery(page, pageSize);
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
