using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance.Repositories;

namespace AllEvents.TicketManagement.InfrastructureTests
{
    public class ReadEventsServiceReaderTests
    {
        private readonly string _validFilePath = "../../../../../../AllEvents.TicketManagement/test/AllEvents.TicketManagement.InfrastructureTests/Data/EventsData.xlsx";
        private readonly string _noDataFilePath = "../../../../../../AllEvents.TicketManagement/test/AllEvents.TicketManagement.InfrastructureTests/Data/NoData.xlsx";
        private readonly string _nonExistentFilePath = "../../../../../../AllEvents.TicketManagement/test/AllEvents.TicketManagement.InfrastructureTests/Data/NonExistent.xlsx";

        [Fact]
        public async Task ReadEventsServiceReaderShouldReturnCorrectNumberOfEvents()
        {
            // Arrange
            var reader = new ReadEventsServiceReader();

            // Act
            var events = await reader.ReadDataFromExcel(_validFilePath);

            // Assert
            Assert.Equal(1000, events.Count);
        }

        [Fact]
        public async Task ReadEventsServiceReaderShouldMapDataCorrectly()
        {
            // Arrange
            var reader = new ReadEventsServiceReader();

            // Act
            var events = await reader.ReadDataFromExcel(_validFilePath);
            var firstEvent = events.First();

            // Assert
            Assert.NotNull(firstEvent.Title);
            Assert.NotNull(firstEvent.Location);
            Assert.True(firstEvent.Price >= 0);
            Assert.True(Enum.IsDefined(typeof(EventCategory), firstEvent.Category));
        }

        [Fact]
        public async Task ReadEventsServiceReaderShouldCheckIfFileExists()
        {
            // Arrange
            var reader = new ReadEventsServiceReader();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(() => reader.ReadDataFromExcel(_nonExistentFilePath));
            Assert.Equal("The specified file does not exist.", exception.Message);
            Assert.Equal(_nonExistentFilePath, exception.FileName);
        }

        [Fact]
        public async Task ReadEventsServiceReaderShouldThrowInvalidOperationExceptionWhenNoData()
        {
            // Arrange
            var reader = new ReadEventsServiceReader();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => reader.ReadDataFromExcel(_noDataFilePath));
            Assert.Equal("The workbook does not contain any data.", exception.Message);
        }

        [Fact]
        public async Task ReadEventsServiceReaderShouldReadDataCorrectly()
        {
            // Arrange
            var reader = new ReadEventsServiceReader();

            // Act
            var events = await reader.ReadDataFromExcel(_validFilePath);

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
