using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllEvents.TicketManagement.App.Pages.Events
{
    public class IndexModel : PageModel
    {
        private readonly IAllEventsDbContext _dbContext;
        public List<Event> Events { get; set; } = new List<Event>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string SelectedTitle { get; set; } = string.Empty;
        public EventCategory? SelectedCategory { get; set; }
        public string? SortBy { get; set; }
        public bool Ascending { get; set; }
        public List<EventCategory> Categories { get; set; } = new List<EventCategory>();

        public IndexModel(IAllEventsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10, string? title = null, EventCategory? category = null, string? sortBy = null, bool ascending = true)
        {
            SelectedTitle = title ?? string.Empty;
            SelectedCategory = category;
            SortBy = sortBy ?? "EventDate";
            Ascending = ascending;
            PageSize = pageSize;
            CurrentPage = pageNumber;

            var query = new EventQuery(_dbContext.Events);

            if (!string.IsNullOrEmpty(title))
            {
                query.Search(title);
            }

            if (category.HasValue)
            {
                query.ForCategory(category.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                query.SortBy(sortBy, ascending);
            }

            var totalEvents = await query.CountAsync();
            Events = await query.ToListAsync(pageNumber - 1, pageSize); 

            TotalPages = (int)Math.Ceiling((double)totalEvents / pageSize);

            Categories = Enum.GetValues(typeof(EventCategory)).Cast<EventCategory>().ToList();
        }
    }
}
