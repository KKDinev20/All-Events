using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.ExternalUsers.Commands;
using AllEvents.TicketManagement.Application.Features.Orders.Commands;
using AllEvents.TicketManagement.Application.Features.Orders.Handlers;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Moq;
using Moq.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Application.UnitTests.Orders
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IAllEventsDbContext> _contextMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _contextMock = new Mock<IAllEventsDbContext>();
            _mediatorMock = new Mock<IMediator>();
            _handler = new CreateOrderCommandHandler(_contextMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task CreateOrderCommandHandler_ThrowsException_WhenMoreThan8Tickets()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                ExternalUserEmail = "test@example.com",
                EventId = Guid.NewGuid(),
                TicketNames = new List<string> { "Ticket1", "Ticket2", "Ticket3", "Ticket4", "Ticket5", "Ticket6", "Ticket7", "Ticket8", "Ticket9" }
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateOrderCommandHandler_ThrowsException_WhenNotEnoughTickets()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var command = new CreateOrderCommand
            {
                ExternalUserEmail = "test@example.com",
                EventId = eventId,
                TicketNames = new List<string> { "Ticket1", "Ticket2" }
            };

            _contextMock.Setup(x => x.Events).ReturnsDbSet(new List<Event>
            {
                new Event { EventId = eventId, NrOfTickets = 1, Price = 100 }
            });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateOrderCommandHandler_ThrowsException_WhenEventDoesNotExist()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                ExternalUserEmail = "test@example.com",
                EventId = Guid.NewGuid(),
                TicketNames = new List<string> { "Ticket1", "Ticket2" }
            };

            _contextMock.Setup(x => x.Events).ReturnsDbSet(new List<Event>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateOrderCommandHandler_SuccessfullyCreatesOrder()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var command = new CreateOrderCommand
            {
                ExternalUserEmail = "test@example.com",
                EventId = eventId,
                TicketNames = new List<string> { "Ticket1", "Ticket2" }
            };

            var eventEntity = new Event { EventId = eventId, NrOfTickets = 10, Price = 100 };
            var externalUser = new ExternalUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Orders = new List<Order>() 
            };

            _contextMock.Setup(x => x.Events)
                .ReturnsDbSet(new List<Event> { eventEntity });

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateExternalUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(externalUser);

            _contextMock.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.TotalAmount);

            _contextMock.Verify(x => x.Orders.Add(It.IsAny<Order>()), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(8, eventEntity.NrOfTickets);
            Assert.Contains(externalUser.Orders, o => o.EventId == eventId && o.TicketNames.Count == 2);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTotalTicketsExceedsLimit()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                EventId = Guid.NewGuid(),
                ExternalUserEmail = "test@example.com",
                TicketNames = new List<string> { "Ticket1", "Ticket2", "Ticket3", "Ticket4", "Ticket5", "Ticket6", "Ticket7", "Ticket8", "Ticket9" }
            };

            var existingOrder = new Order
            {
                TicketNames = new List<string> { "ExistingTicket1", "ExistingTicket2" },
                ExternalUserId = Guid.NewGuid()
            };

            _contextMock.Setup(c => c.Orders)
                .ReturnsDbSet(new List<Order> { existingOrder }.AsQueryable());

            _contextMock.Setup(c => c.Events)
                .ReturnsDbSet(new List<Event>
                {
                new Event { EventId = command.EventId, NrOfTickets = 10 }
                }.AsQueryable());

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateExternalUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExternalUser { Id = existingOrder.ExternalUserId });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Cannot order more than 8 tickets per event.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldUpdateTicketCount_WhenOrderIsCreated()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                EventId = Guid.NewGuid(),
                ExternalUserEmail = "test@example.com",
                TicketNames = new List<string> { "Ticket1", "Ticket2" }
            };

            var eventEntity = new Event
            {
                EventId = command.EventId,
                NrOfTickets = 10,
                Price = 20
            };

            _contextMock.Setup(c => c.Events)
                .ReturnsDbSet(new List<Event> { eventEntity }.AsQueryable());

            _contextMock.Setup(c => c.Orders)
                .ReturnsDbSet(new List<Order>().AsQueryable());

            var externalUser = new ExternalUser { Id = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateExternalUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(externalUser);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(2 * eventEntity.Price, response.TotalAmount);

            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
