using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Domain.Entities;
using OfficeOpenXml;
using System.Globalization;

namespace AllEvents.TicketManagement.Persistance.Repositories
{
    public class ReadEventsServiceReader : IEventsDataSeeder
    {
        public async Task<List<Event>> ReadDataFromExcel(string filePath)
        {
            ValidateFilePath(filePath);

            using var reader = new ExcelPackage(new FileInfo(filePath));
            var worksheet = GetWorksheet(reader);

            return await ParseEventsFromWorksheetAsync(worksheet);
        }

        private void ValidateFilePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file does not exist.", filePath);
            }
        }

        private ExcelWorksheet GetWorksheet(ExcelPackage reader)
        {
            if (reader.Workbook.Worksheets.Count == 0)
            {
                throw new InvalidOperationException("The workbook does not contain any worksheets.");
            }

            var worksheet = reader.Workbook.Worksheets.First();

            if (worksheet.Dimension == null || worksheet.Dimension.Rows < 2)
            {
                throw new InvalidOperationException("The workbook does not contain any data.");
            }

            return worksheet;
        }

        private async Task<List<Event>> ParseEventsFromWorksheetAsync(ExcelWorksheet worksheet)
        {
            var events = new List<Event>();
            var rows = worksheet.Dimension.Rows;

            await Task.Yield();

            for (int row = 2; row <= rows; row++)
            {
                var eventItem = ParseEventFromRow(worksheet, row);
                events.Add(eventItem);
            }

            return events;
        }

        private Event ParseEventFromRow(ExcelWorksheet worksheet, int row)
        {
            var title = worksheet.Cells[row, 1].Value?.ToString();
            var location = worksheet.Cells[row, 2].Value?.ToString();
            var price = ParsePrice(worksheet.Cells[row, 3].Value?.ToString());
            var category = ParseCategory(worksheet.Cells[row, 4].Value?.ToString());
            var eventDate = ParseEventDate(worksheet.Cells[row, 5].Value?.ToString());

            return new Event
            {
                EventId = Guid.NewGuid(),
                Title = title,
                Location = location,
                Price = price,
                Category = category,
                EventDate = eventDate
            };
        }

        private decimal ParsePrice(string priceStr)
        {
            if (!decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            {
                throw new FormatException($"The input string '{priceStr}' was not in a correct format for decimal.");
            }

            return price;
        }

        private EventCategory ParseCategory(string categoryStr)
        {
            if (!Enum.TryParse(categoryStr, true, out EventCategory category))
            {
                category = EventCategory.Other;
            }

            return category;
        }

        private DateTime ParseEventDate(string eventDateStr)
        {
            if (!DateTime.TryParse(eventDateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var eventDate))
            {
                throw new FormatException($"The input string '{eventDateStr}' was not in a correct format for DateTime.");
            }

            return eventDate;
        }
    }
}
