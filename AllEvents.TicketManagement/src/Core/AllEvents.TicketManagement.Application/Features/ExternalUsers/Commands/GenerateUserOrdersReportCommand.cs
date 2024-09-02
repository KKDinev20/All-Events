using MediatR;

namespace AllEvents.TicketManagement.Application.Reports.Commands
{
    public class GenerateUserOrdersReportCommand : IRequest<byte[]>
    {
        public string ExternalUserEmail { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public GenerateUserOrdersReportCommand(string externalUserEmail, DateTime? fromDate = null, DateTime? toDate = null)
        {
            ExternalUserEmail = externalUserEmail;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}
