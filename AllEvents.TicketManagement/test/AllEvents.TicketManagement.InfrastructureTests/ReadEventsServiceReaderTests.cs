using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using AllEvents.TicketManagement.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            Assert.True(firstEvent.Price > 0);
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

<<<<<<< HEAD
            // Assert
            Assert.Equal("The specified file does not exist.", exception.Message);
            Assert.Equal(filePath, exception.FileName);
=======
            //Assert
            Assert.Equal("The specified file does not exist.", exception.Message);
            Assert.Equal(filePath, exception.FileName); // You created this one as FileName obviously it won't be in the message
>>>>>>> 4fd22a12496745728c20dae434dadc973d35e001
        }

        [Fact]
        public async Task ReadEventsServiceReaderShouldReadDataCorrectly()
        {
            // Arrange
            var filePath = "../../../../../../AllEvents.TicketManagement/src/Infrastructure/AllEvents.TicketManagement.Persistance/Data/EventsData.xlsx";
            var reader = new ReadEventsServiceReader();

            // Act and Assert
            var events = await reader.ReadAndSeedDataFromExcel(filePath);

<<<<<<< HEAD
            // Assert
            Assert.NotEmpty(events);
            Assert.All(events, eventItem =>
            {
                Assert.NotNull(eventItem.Title);
                Assert.NotNull(eventItem.Location);
                Assert.True(eventItem.Price >= 0); 
                Assert.True(Enum.IsDefined(typeof(EventCategory), eventItem.Category));
            });
=======
            //Assert 
            // the file which you are opening contains worksheets
            // add one that does not in the test project
            Assert.Equal("The workbook does not contain any worksheets.", exception.Message);
>>>>>>> 4fd22a12496745728c20dae434dadc973d35e001
        }

        [Fact]
        public async Task ReadEventsServiceReaderShouldThrowInvalidOperationExceptionWhenNoData()
        {
            // Arrange
            var filePath = "../../../../../../AllEvents.TicketManagement/src/Infrastructure/AllEvents.TicketManagement.Persistance/Data/NoData.xlsx";
            var reader = new ReadEventsServiceReader();

            // Act 
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => reader.ReadAndSeedDataFromExcel(filePath));

<<<<<<< HEAD
            // Assert
=======
            //Assert
            // the file which you are opening contains data
            // add one that does not in the test project
>>>>>>> 4fd22a12496745728c20dae434dadc973d35e001
            Assert.Equal("The workbook does not contain any data.", exception.Message);
        }
    }
}