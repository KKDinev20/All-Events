using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Reports.Commands;
using AllEvents.TicketManagement.Domain.Entities;
using Moq;

namespace AllEvents.TicketManagement.ApplicationTests
{
    public class GenerateUserOrdersReportCommandHandlerTests
    {
        private readonly Mock<IExternalUserRepository> _externalUserRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly GenerateUserOrdersReportCommandHandler _handler;

        public GenerateUserOrdersReportCommandHandlerTests()
        {
            _externalUserRepositoryMock = new Mock<IExternalUserRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();

            _handler = new GenerateUserOrdersReportCommandHandler(
                _externalUserRepositoryMock.Object,
                _orderRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPdfByteArray_WhenOrdersExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var externalUserEmail = "test@example.com";
            var orders = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    Status = OrderStatus.Processing,
                    Event = new Event { Title = "Test Event", Location = "Test Location" },
                    TicketNames = new List<string> { "Ticket1", "Ticket2" }
                }
            };

            _externalUserRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(externalUserEmail))
                .ReturnsAsync(new ExternalUser { Id = userId });

            _orderRepositoryMock.Setup(repo => repo.GetOrdersByUserAndDateRangeAsync(userId, null, null))
                .ReturnsAsync(orders);

            // Act
            var result = await _handler.Handle(new GenerateUserOrdersReportCommand(externalUserEmail, null, null), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0, "PDF byte array should not be empty");
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            // Arrange
            var externalUserEmail = "nonexistent@example.com";

            _externalUserRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(externalUserEmail))
                .ReturnsAsync((ExternalUser)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(new GenerateUserOrdersReportCommand(externalUserEmail, null, null), CancellationToken.None));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenNoOrdersFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var externalUserEmail = "test@example.com";

            _externalUserRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(externalUserEmail))
                .ReturnsAsync(new ExternalUser { Id = userId });

            _orderRepositoryMock.Setup(repo => repo.GetOrdersByUserAndDateRangeAsync(userId, null, null))
                .ReturnsAsync(new List<Order>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(new GenerateUserOrdersReportCommand(externalUserEmail, null, null), CancellationToken.None));
            Assert.Equal("No orders found for the specified user and date range", exception.Message);
        }
    }
}
