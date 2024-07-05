using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Persistance.Repositories
{
    public class ReadEventsServiceReader : IEventsDataSeeder
    {
        public async Task<List<Event>> ReadAndSeedDataFromExcel(string filePath)
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
                    var eventItem = new Event
                    {
                        EventId = Guid.NewGuid(),
                        Title = worksheet.Cells[row, 2].Value?.ToString(),
                        Location = worksheet.Cells[row, 3].Value?.ToString(),
                        Price = decimal.Parse(worksheet.Cells[row, 4].Value?.ToString()),
                        Category = Enum.TryParse<EventCategory>(worksheet.Cells[row, 5].Value?.ToString(), out var category)
                                ? category
                                : EventCategory.Other
                    };

                    events.Add(eventItem);
                }
            }

            return events;

        }
    }
}
