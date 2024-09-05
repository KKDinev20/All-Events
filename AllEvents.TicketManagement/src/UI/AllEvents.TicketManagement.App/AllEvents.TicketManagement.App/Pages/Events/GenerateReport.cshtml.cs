using AllEvents.TicketManagement.Application.Reports.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

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
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "From date is required")]
        public DateTime FromDate { get; set; }

        [BindProperty]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "To date is required")]
        public DateTime ToDate { get; set; }

        public byte[] ReportCsvData { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Invalid input data.";
                return Page();
            }

            if (FromDate > ToDate)
            {
                ErrorMessage = "From date must be earlier than or equal to To date.";
                return Page();
            }

            try
            {
                var command = new GenerateCategorySalesReportCommand(Category, FromDate, ToDate);
                ReportCsvData = await _mediator.Send(command);

                if (ReportCsvData != null && ReportCsvData.Length > 0)
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMdd"); 
                    var fileName = $"SalesReport_{Category}_{timestamp}.csv";

                    return File(ReportCsvData, "text/csv", fileName);
                }
                else
                {
                    ErrorMessage = "No data available for the specified parameters.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while generating the report: {ex.Message}";
            }

            return Page();
        }
    }
}
