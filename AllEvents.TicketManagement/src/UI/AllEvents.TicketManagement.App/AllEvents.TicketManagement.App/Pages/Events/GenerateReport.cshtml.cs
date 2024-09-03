using AllEvents.TicketManagement.Application.Reports.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AllEvents.TicketManagement.Web.Pages
{
    public class GenerateReportModel : PageModel
    {
        private readonly IMediator _mediator;

        public GenerateReportModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public EventCategory Category { get; set; }
        [BindProperty]
        public DateTime? FromDate { get; set; }
        [BindProperty]
        public DateTime? ToDate { get; set; }
        public byte[] ReportCsvData { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var command = new GenerateCategorySalesReportCommand(Category, FromDate, ToDate);
                ReportCsvData = await _mediator.Send(command);

                if (ReportCsvData != null && ReportCsvData.Length > 0)
                {
                    var timestamp = DateTime.Now.ToString("yyyyddMM");
                    var fileName = $"SalesReport_{Category}_{timestamp}.csv";

                    return File(ReportCsvData, "text/csv", fileName);
                }
            }

            return Page();
        }
    }
}
