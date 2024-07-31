using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AllEvents.TicketManagement.Tests
{
    public class UpdateEventCommandHandlerTests
    {
        private readonly Mock<IAllEventsDbContext> _mockDbContext;
        private readonly Mock<IValidator<UpdateEventCommand>> _mockValidator;
        private readonly UpdateEventCommandHandler _handler;
        private readonly List<Event> _events;

        public UpdateEventCommandHandlerTests()
        {
            _mockDbContext = new Mock<IAllEventsDbContext>();
            _mockValidator = new Mock<IValidator<UpdateEventCommand>>();

            _events = new List<Event>
            {
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Existing Event",
                    Location = "Existing Location",
                    Price = 50.00m,
                    Category = EventCategory.Sports,
                    EventDate = DateTime.Now.AddDays(20),
                    NrOfTickets = 100
                }
            };

            var mockEventDbSet = new Mock<DbSet<Event>>();
            mockEventDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(_events.AsQueryable().Provider);
            mockEventDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(_events.AsQueryable().Expression);
            mockEventDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(_events.AsQueryable().ElementType);
            mockEventDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(_events.AsQueryable().GetEnumerator());

            _mockDbContext.Setup(db => db.Events).Returns(mockEventDbSet.Object);

            _handler = new UpdateEventCommandHandler(_mockDbContext.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenTitleIsMissing()
        {
            // Arrange
            var existingEvent = _events[0];
            var command = new UpdateEventCommand
            {
                EventId = existingEvent.EventId,
                Title = "",
                Location = "Updated Location",
                Price = 60.00m,
                Category = EventCategory.Music,
                EventDate = DateTime.Now.AddDays(30),
                NrOfTickets = 150
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Title", "Title is required")
            };
            var validationResult = new ValidationResult(validationFailures);

            _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Validation failed: \n -- Title: Title is required Severity: Error");
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenEventDateIsInPast()
        {
            // Arrange
            var existingEvent = _events[0];
            var command = new UpdateEventCommand
            {
                EventId = existingEvent.EventId,
                Title = "Updated Title",
                Location = "Updated Location",
                Price = 60.00m,
                Category = EventCategory.Music,
                EventDate = DateTime.Now.AddDays(-1),
                NrOfTickets = 150
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("EventDate", "Event date must be in the future.")
            };
            var validationResult = new ValidationResult(validationFailures);

            _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Validation failed: \n -- EventDate: Event date must be in the future. Severity: Error");
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenPriceIsNegative()
        {
            // Arrange
            var existingEvent = _events[0];
            var command = new UpdateEventCommand
            {
                EventId = existingEvent.EventId,
                Title = "Updated Title",
                Location = "Updated Location",
                Price = -10.00m,
                Category = EventCategory.Music,
                EventDate = DateTime.Now.AddDays(30),
                NrOfTickets = 150
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Price", "Price must be a positive value.")
            };
            var validationResult = new ValidationResult(validationFailures);

            _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Validation failed: \n -- Price: Price must be a positive value. Severity: Error");
        }


        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()

        {
            // Arrange
            var existingEvent = _events[0];
            var command = new UpdateEventCommand
            {
                EventId = existingEvent.EventId,
                Title = "Updated Title",
                Location = "Updated Location",
                Price = 60.00m,
                Category = EventCategory.Music,
                EventDate = DateTime.Now.AddDays(30),
                NrOfTickets = 150
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Title", "Title is required")
            };
            var validationResult = new ValidationResult(validationFailures);

            _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Validation failed: \n -- Title: Title is required Severity: Error");
        }


    }
}
