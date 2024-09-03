using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Application.Reports.Commands
{
    public class GenerateCategorySalesReportCommandHandler : IRequestHandler<GenerateCategorySalesReportCommand, byte[]>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventRepository _eventRepository;

        public GenerateCategorySalesReportCommandHandler(
            IOrderRepository orderRepository,
            IEventRepository eventRepository)
        {
            _orderRepository = orderRepository;
            _eventRepository = eventRepository;
        }

        public async Task<byte[]> Handle(GenerateCategorySalesReportCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventRepository.GetEventsByCategoryAndDateRangeAsync(request.EventCategory, request.FromDate, request.ToDate);

            if (events == null || !events.Any())
            {
                throw new Exception("No events found for the specified criteria");
            }

            var eventIds = events.Select(e => e.EventId).ToList();
            var orders = await _orderRepository.GetOrdersByEventIdsAsync(eventIds);

            var reportLines = new List<string>();
            var header = "Event Title,Event Location,Tickets Sold,Amount per Ticket,Total per Event";
            reportLines.Add(header);

            var categoryTotal = 0M;

            foreach (var ev in events)
            {
                var ticketsSold = orders.Count(o => o.EventId == ev.EventId && o.Status == OrderStatus.Processing);

                if (ticketsSold > 0) 
                {
                    var amountPerEvent = ev.Price * ticketsSold;
                    categoryTotal += amountPerEvent;

                    var line = $"{EscapeCsvField(ev.Title)},{EscapeCsvField(ev.Location)},{ticketsSold},{ev.Price.ToString(CultureInfo.InvariantCulture)},{amountPerEvent.ToString(CultureInfo.InvariantCulture)}";
                    reportLines.Add(line);
                }
            }

            var totalLine = $",,,,{categoryTotal.ToString(CultureInfo.InvariantCulture)}";
            reportLines.Add(totalLine);

            return System.Text.Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, reportLines));
        }

        private string EscapeCsvField(string field)
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
    }
}
