using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllEvents.TicketManagement.App.Pages.Events
{
    public class IndexModel : PageModel
    {
        private readonly IEventRepository _eventRepository;
        public List<Event> Events { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string SelectedTitle { get; set; } = string.Empty;
        public EventCategory? SelectedCategory { get; set; }
        public string? SortBy { get; set; }
        public bool Ascending { get; set; }
        public List<EventCategory> Categories { get; set; } = new List<EventCategory>();

        public IndexModel(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10, string? title = null, EventCategory? category = null, string? sortBy = null, bool ascending = true)
        {
            SelectedTitle = title ?? string.Empty;
            SelectedCategory = category;
            SortBy = sortBy ?? "EventDate";
            Ascending = ascending;

            var totalEvents = await _eventRepository.GetFilteredCountAsync(title, category);
            Events = await _eventRepository.GetFilteredPagedEventsAsync(pageNumber, pageSize, title, category, sortBy, ascending);
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)System.Math.Ceiling((double)totalEvents / pageSize);

            Categories = Enum.GetValues(typeof(EventCategory)).Cast<EventCategory>().ToList();
        }
    }

}
