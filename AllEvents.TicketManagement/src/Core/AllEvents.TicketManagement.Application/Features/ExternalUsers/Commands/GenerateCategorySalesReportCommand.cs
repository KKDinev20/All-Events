using AllEvents.TicketManagement.Domain.Entities;
using MediatR;

namespace AllEvents.TicketManagement.Application.Reports.Commands
{
    public class GenerateCategorySalesReportCommand : IRequest<byte[]>
    {
        public EventCategory EventCategory { get; }
        public DateTime? FromDate { get; }
        public DateTime? ToDate { get; }

        public GenerateCategorySalesReportCommand(EventCategory eventCategory, DateTime? fromDate, DateTime? toDate)
        {
            EventCategory = eventCategory;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}
