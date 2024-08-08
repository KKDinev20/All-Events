using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace AllEvents.TicketManagement.Tests
{
    public class CreateEventCommandHandlerTests
    {
        private readonly Mock<IAllEventsDbContext> _mockDbContext;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly CreateEventCommandHandler _handler;

        public CreateEventCommandHandlerTests()
        {
            _mockDbContext = new Mock<IAllEventsDbContext>();
            _mockCache = new Mock<IDistributedCache>(); 

            var mockEventDbSet = new Mock<DbSet<Event>>();
            _mockDbContext.Setup(db => db.Events).Returns(mockEventDbSet.Object);

            _handler = new CreateEventCommandHandler(_mockDbContext.Object, _mockCache.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateEvent_WhenCommandIsValid()
        {
            // Arrange
            var command = new CreateEventCommand
            {
                Title = "Test Event",
                Location = "Test Location",
                Price = 25.00m,
                Category = EventCategory.Music,
                EventDate = DateTime.Now.AddDays(10),
                NrOfTickets = 200
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockDbContext.Verify(db => db.Events.Add(It.IsAny<Event>()), Times.Once);
            _mockDbContext.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
            result.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenDbContextFails()
        {
            // Arrange
            var command = new CreateEventCommand
            {
                Title = "Test Event",
                Location = "Test Location",
                Price = 25.00m,
                Category = EventCategory.Music,
                EventDate = DateTime.Now.AddDays(10),
                NrOfTickets = 200
            };

            _mockDbContext.Setup(db => db.SaveChangesAsync(CancellationToken.None))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
        }

        [Fact]
        public async Task Handle_ShouldMapFieldsCorrectly()
        {
            // Arrange
            var command = new CreateEventCommand
            {
                Title = "Test Event",
                Location = "Test Location",
                Price = 25.00m,
                Category = EventCategory.Music,
                EventDate = DateTime.Now.AddDays(10),
                NrOfTickets = 200
            };

            Event addedEvent = null;
            _mockDbContext.Setup(db => db.Events.Add(It.IsAny<Event>())).Callback<Event>(ev => addedEvent = ev);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            addedEvent.Should().NotBeNull();
            addedEvent.Title.Should().Be(command.Title);
            addedEvent.Location.Should().Be(command.Location);
            addedEvent.Price.Should().Be(command.Price);
            addedEvent.Category.Should().Be(command.Category);
            addedEvent.EventDate.Should().Be(command.EventDate);
            addedEvent.NrOfTickets.Should().Be(command.NrOfTickets);
            addedEvent.IsDeleted.Should().BeFalse();
        }
    }
}
