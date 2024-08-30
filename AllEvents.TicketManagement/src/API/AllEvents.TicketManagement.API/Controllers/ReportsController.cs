using AllEvents.TicketManagement.Application.Reports.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllEvents.TicketManagement.API.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("user-report")]
        public async Task<IActionResult> GetUserOrdersReport(string externalUserEmail, DateTime? fromDate, DateTime? toDate)
        {
            var command = new GenerateUserOrdersReportCommand(externalUserEmail, fromDate, toDate);
            var pdfBytes = await _mediator.Send(command);

            return File(pdfBytes, "application/pdf", "UserReport.pdf");
        }

    }
}
