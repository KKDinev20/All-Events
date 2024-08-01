using AllEvents.TicketManagement.Application.Features.Tickets.Commands;
using AllEvents.TicketManagement.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllEvents.TicketManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IMediator mediator;

        public TicketController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<TicketModel>> GenerateTicket([FromBody] GenerateTicketCommand command)
        {
            var ticket = await mediator.Send(command);
            return Ok(ticket);
        }

        [HttpGet("validate")]
        public async Task<IActionResult> Validate([FromQuery] string token)
        {
            var command = new ValidateTicketCommand { Token = token };
            var result = await mediator.Send(command);

            if (result.IsSuccessful)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}