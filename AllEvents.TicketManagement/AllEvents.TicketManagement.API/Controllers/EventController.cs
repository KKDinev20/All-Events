using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {

        private readonly AllEventsDbContext allEventsDbContext;

        public EventController(AllEventsDbContext allEventsDbContext)
        {
            this.allEventsDbContext = allEventsDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<Event>>> RetrieveAllEvents(int page = 1, int pageSize = 10)
        {
            var totalCount = await allEventsDbContext.Events.CountAsync();

            var items = await allEventsDbContext.Events
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return Ok(new PagedResult<Event>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }
    }
}
