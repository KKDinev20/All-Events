using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.App.Pages.Events
{
    public class IndexModel : PageModel
    {
        private readonly IEventRepository _eventRepository;
        public List<Event> Events { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public IndexModel(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
            var totalEvents = await _eventRepository.GetCountAsync();
            Events = await _eventRepository.GetPagedEventsAsync(pageNumber - 1, pageSize, includeDeleted: true);
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)System.Math.Ceiling((double)totalEvents / pageSize);
        }
    }
}
