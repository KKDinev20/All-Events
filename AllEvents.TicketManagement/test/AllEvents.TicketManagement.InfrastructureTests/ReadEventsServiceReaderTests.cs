using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance.Repositories;

namespace AllEvents.TicketManagement.InfrastructureTests
{
    public class ReadEventsServiceReaderTests
    {
        [Fact]
        public async Task ReadEventsServiceReaderShouldReturnCorrectNumberOfEvents()
        {
            // Arrange
            var filePath = "../../../../../../AllEvents.TicketManagement\\src\\Infrastructure\\AllEvents.TicketManagement.Persistance\\Data\\EventsData.xlsx";
            var reader = new ReadEventsServiceReader();

            // Act
            var events = await reader.ReadAndSeedDataFromExcel(filePath);

            // Assert
            Assert.Equal(1000, events.Count); 
        }

        [Fact]
        public async Task ReadEventsServiceReader_ShouldMapDataCorrectly()
        {
            // Arrange
            var filePath = "../../../../../../AllEvents.TicketManagement\\src\\Infrastructure\\AllEvents.TicketManagement.Persistance\\Data\\EventsData.xlsx";
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
    }
}