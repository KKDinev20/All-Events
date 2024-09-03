using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Reports.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using Moq;

namespace AllEvents.TicketManagement.ApplicationTests
{
    public class GenerateCategorySalesReportCommandHandlerTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly GenerateCategorySalesReportCommandHandler _handler;

        public GenerateCategorySalesReportCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _handler = new GenerateCategorySalesReportCommandHandler(_orderRepositoryMock.Object, _eventRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_NoEventsFound_ThrowsException()
        {
            // Arrange
            var command = new GenerateCategorySalesReportCommand(
                eventCategory: EventCategory.Music,
                fromDate: DateTime.UtcNow.AddDays(-30),
                toDate: DateTime.UtcNow
            );

            _eventRepositoryMock.Setup(repo => repo.GetEventsByCategoryAndDateRangeAsync(It.IsAny<EventCategory>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<Event>());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NoOrdersFound_ReturnsEmptyReportWithHeader()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { EventId = Guid.NewGuid(), Title = "Test Event", Location = "Test Location", Price = 50, Category = EventCategory.Music }
            };

            var command = new GenerateCategorySalesReportCommand(
                eventCategory: EventCategory.Music,
                fromDate: DateTime.UtcNow.AddDays(-30),
                toDate: DateTime.UtcNow
            );

            _eventRepositoryMock.Setup(repo => repo.GetEventsByCategoryAndDateRangeAsync(It.IsAny<EventCategory>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(events);

            _orderRepositoryMock.Setup(repo => repo.GetTicketCountsByEventIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new Dictionary<Guid, int>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            var resultString = System.Text.Encoding.UTF8.GetString(result);
            var lines = resultString.Split(Environment.NewLine);

            // Assert
            Assert.Equal(2, lines.Length);
            Assert.Equal("Event Title,Event Location,Tickets Sold,Amount per Ticket,Total per Event", lines[0]);
            Assert.Equal(",,,,0", lines[1]);
        }


        [Fact]
        public async Task Handle_ValidReport_ReturnsCorrectCsv()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { EventId = Guid.NewGuid(), Title = "Test Event 1", Location = "Location 1", Price = 50, Category = EventCategory.Music },
                new Event { EventId = Guid.NewGuid(), Title = "Test Event 2", Location = "Location 2", Price = 100, Category = EventCategory.Music }
            };

            var orders = new Dictionary<Guid, int>
            {
                { events[0].EventId, 2 },
                { events[1].EventId, 3 }
            };

            var command = new GenerateCategorySalesReportCommand(
                eventCategory: EventCategory.Music,
                fromDate: DateTime.UtcNow.AddDays(-30),
                toDate: DateTime.UtcNow
            );

            _eventRepositoryMock.Setup(repo => repo.GetEventsByCategoryAndDateRangeAsync(It.IsAny<EventCategory>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(events);

            _orderRepositoryMock.Setup(repo => repo.GetTicketCountsByEventIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(orders);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            var resultString = System.Text.Encoding.UTF8.GetString(result);
            var lines = resultString.Split(Environment.NewLine);

            string expectedLineEvent1 = "\"Test Event 1\",\"Location 1\",2,50,100";
            string expectedLineEvent2 = "\"Test Event 2\",\"Location 2\",3,100,300";

            // Assert
            Assert.Equal(4, lines.Length);
            Assert.Equal("Event Title,Event Location,Tickets Sold,Amount per Ticket,Total per Event", lines[0]);
            Assert.Contains(expectedLineEvent1, lines[1]);
            Assert.Contains(expectedLineEvent2, lines[2]);
            Assert.Equal(",,,,400", lines[3]);
        }


    }
}
