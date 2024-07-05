using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllEvents.TicketManagement.InfrastructureTests
{
    public class ReadEventsServiceReaderTests
    {
        [Fact]
        public async Task ReadEventsServiceReaderShouldReturnCorrectNumberOfEvents()
        {
            // Arrange
            var filePath = "../../../../../../AllEvents.TicketManagement/src/Infrastructure/AllEvents.TicketManagement.Persistance/Data/EventsData.xlsx";
            var reader = new ReadEventsServiceReader();

            // Act
            var events = await reader.ReadAndSeedDataFromExcel(filePath);

            // Assert
            Assert.Equal(1000, events.Count);
        }

        [Fact]
        public async Task ReadEventsServiceReaderShouldMapDataCorrectly()
        {
            // Arrange
            var filePath = "../../../../../../AllEvents.TicketManagement/src/Infrastructure/AllEvents.TicketManagement.Persistance/Data/EventsData.xlsx";
            var reader = new ReadEventsServiceReader();

            // Act
            var events = await reader.ReadAndSeedDataFromExcel(filePath);
            var firstEvent = events.First();

            // Assert
            Assert.NotNull(firstEvent.Title);
            Assert.NotNull(firstEvent.Location);
            Assert.True(firstEvent.Price >= 0);
            Assert.IsType<EventCategory>(firstEvent.Category);
        }

        [Fact]
        public async Task ReadEventsServiceReaderShouldCheckIfFileExists()
        {
            // Arrange
            var filePath = "nonexistent.xlsx";
            var reader = new ReadEventsServiceReader();

            // Act
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(() => reader.ReadAndSeedDataFromExcel(filePath));

            // Assert
            Assert.Equal("The specified file does not exist.", exception.Message);
            Assert.Equal(filePath, exception.FileName);
        }

        //It won't work properly, because an excel file always has atleast one worksheet.
       /* [Fact]
        public async Task ReadEventsServiceReaderShouldThrowInvalidOperationExceptionWhenNoWorksheets()
        {
            // Arrange
            var filePath = "../../../../../../AllEvents.TicketManagement/src/Infrastructure/AllEvents.TicketManagement.Persistance/Data/NoWorksheets.xlsx"; // Path to a file with no worksheets
            var reader = new ReadEventsServiceReader();

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => reader.ReadAndSeedDataFromExcel(filePath));

            // Assert
            Assert.Equal("The workbook does not contain any worksheets.", exception.Message);
        }*/

        [Fact]
        public async Task ReadEventsServiceReaderShouldThrowInvalidOperationExceptionWhenNoData()
        {
            // Arrange
            var filePath = "../../../../../../AllEvents.TicketManagement/src/Infrastructure/AllEvents.TicketManagement.Persistance/Data/NoData.xlsx"; // Path to a file with no data
            var reader = new ReadEventsServiceReader();

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => reader.ReadAndSeedDataFromExcel(filePath));

            // Assert
            Assert.Equal("The workbook does not contain any data.", exception.Message);
        }

        [Fact]
        public async Task ReadEventsServiceReaderShouldReadDataCorrectly()
        {
            // Arrange
            var filePath = "../../../../../../AllEvents.TicketManagement/src/Infrastructure/AllEvents.TicketManagement.Persistance/Data/EventsData.xlsx";
            var reader = new ReadEventsServiceReader();

            // Act
            var events = await reader.ReadAndSeedDataFromExcel(filePath);

            // Assert
            Assert.NotEmpty(events);
            Assert.All(events, eventItem =>
            {
                Assert.NotNull(eventItem.Title);
                Assert.NotNull(eventItem.Location);
                Assert.True(eventItem.Price >= 0);
                Assert.True(Enum.IsDefined(typeof(EventCategory), eventItem.Category));
            });
        }
    }
}
