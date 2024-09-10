using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.App.Pages.Coupons
{
    public class ManageCouponsModel : PageModel
    {
        private readonly IAllEventsDbContext _context;

        public ManageCouponsModel(IAllEventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Coupon Coupon { get; set; } = new Coupon();

        public IList<Coupon> Coupons { get; set; } = new List<Coupon>();
        public IList<Event> Events { get; set; } = new List<Event>();

        public async Task OnGetAsync()
        {
            Coupons = await _context.Coupons
                .Include(c => c.Event)
                .ToListAsync();

            Events = await _context.Events.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Coupons.Add(Coupon);
            await _context.SaveChangesAsync(cancellationToken);

            return RedirectToPage();
        }
    }
}
