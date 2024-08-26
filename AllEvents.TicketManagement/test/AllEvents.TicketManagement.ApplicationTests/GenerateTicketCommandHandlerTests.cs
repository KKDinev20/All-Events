using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Tickets.Commands;
using AllEvents.TicketManagement.Application.Features.Tickets.Handlers;
using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AllEvents.TicketManagement.Tests
{
    public class GenerateTicketCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ITicketRepository> _mockTicketRepository;
        private readonly Mock<IEventRepository> _mockEventRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly GenerateTicketCommandHandler _handler;

        public GenerateTicketCommandHandlerTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockEventRepository = new Mock<IEventRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.SetupGet(x => x["Security:AES_Key"]).Returns("0123456789ABCDEF");
            _mockConfiguration.SetupGet(x => x["Security:AES_IV"]).Returns("0123456789ABCDEF");

            _handler = new GenerateTicketCommandHandler(
                _mockConfiguration.Object,
                _mockOrderRepository.Object,
                _mockTicketRepository.Object,
                _mockEventRepository.Object
            );
        }

        [Fact]
        public async Task Handle_OrderNotFound_ThrowsArgumentException()
        {
            // Arrange
            var command = new GenerateTicketCommand { OrderId = Guid.NewGuid(), EventId = Guid.NewGuid(), PersonName = "John Doe" };
            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Order with ID {command.OrderId} not found.", exception.Message);
        }

        [Fact]
        public async Task Handle_InvalidOrderState_ThrowsInvalidOperationException()
        {
            // Arrange
            var order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Created };
            var command = new GenerateTicketCommand { OrderId = order.Id, EventId = Guid.NewGuid(), PersonName = "John Doe" };
            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(order);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Tickets can only be generated for orders in the 'Processing' state.", exception.Message);
        }

        [Fact]
        public async Task Handle_NameNotFoundInOrder_ThrowsArgumentException()
        {
            // Arrange
            var order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Processing, TicketNames = new List<string> { "Jane Doe" } };
            var command = new GenerateTicketCommand { OrderId = order.Id, EventId = Guid.NewGuid(), PersonName = "John Doe" };
            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(order);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("The specified name does not match any names in the order.", exception.Message);
        }

        [Fact]
        public async Task Handle_EventNotFound_ThrowsArgumentException()
        {
            // Arrange
            var order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Processing, TicketNames = new List<string> { "John Doe" } };
            var command = new GenerateTicketCommand { OrderId = order.Id, EventId = Guid.NewGuid(), PersonName = "John Doe" };
            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(order);
            _mockEventRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Event)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Event with ID {command.EventId} not found.", exception.Message);
        }
    }
}
