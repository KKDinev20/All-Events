using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Persistance.Repositories
{
    public class ReadEventsServiceReader : IEventsDataSeeder
    {
        public async Task<List<Event>> ReadDataFromExcel(string filePath)
        {
            var events = new List<Event>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file does not exist.", filePath);
            }


            using (var reader = new ExcelPackage(new FileInfo(filePath)))
            {
                await Task.Yield();
                var worksheet = reader.Workbook.Worksheets.First(); 

                if(reader.Workbook.Worksheets.Count == 0)
                {
                    throw new InvalidOperationException("The workbook does not contain any worksheets.");
                }

                var rows = worksheet.Dimension.Rows;

                if (worksheet.Dimension == null || worksheet.Dimension.Rows < 2)
                {
                    throw new InvalidOperationException("The workbook does not contain any data.");
                }

                for (int row = 2; row <= rows; row++)
                {
                    var title = worksheet.Cells[row, 1].Value?.ToString();
                    var location = worksheet.Cells[row, 2].Value?.ToString();
                    var priceStr = worksheet.Cells[row, 3].Value?.ToString();
                    var categoryStr = worksheet.Cells[row, 4].Value?.ToString();
                    var eventDateStr = worksheet.Cells[row, 5].Value?.ToString();

                    decimal price;
                    if (!decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                    {
                        throw new FormatException($"The input string '{priceStr}' was not in a correct format for decimal.");
                    }

                    EventCategory category;
                    if (!Enum.TryParse(categoryStr, true, out category))
                    {
                        category = EventCategory.Other;
                    }

                    DateTime eventDate;
                    if (!DateTime.TryParse(eventDateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out eventDate))
                    {
                        throw new FormatException($"The input string '{eventDateStr}' was not in a correct format for DateTime.");
                    }

                    var eventItem = new Event
                    {
                        EventId = Guid.NewGuid(),
                        Title = title,
                        Location = location,
                        Price = price,
                        Category = category,
                        EventDate = eventDate
                    };

                    events.Add(eventItem);
                }
            }

            return events;

        }
    }
}
