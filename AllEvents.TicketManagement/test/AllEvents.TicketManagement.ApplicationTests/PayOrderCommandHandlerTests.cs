using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Orders.Commands;
using AllEvents.TicketManagement.Application.Features.Orders.Handlers;
using AllEvents.TicketManagement.Domain.Entities;
using Moq;
using Moq.EntityFrameworkCore;

namespace AllEvents.TicketManagement.ApplicationTests
{
    public class PayOrderCommandHandlerTests
    {
        private readonly Mock<IAllEventsDbContext> _contextMock;
        private readonly PayOrderCommandHandler _handler;

        public PayOrderCommandHandlerTests()
        {
            _contextMock = new Mock<IAllEventsDbContext>();
            _handler = new PayOrderCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Handle_OrderDoesNotExist_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new PayOrderCommand(Guid.NewGuid());

            _contextMock.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_OrderNotInCreatedState_ThrowsInvalidOperationException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Completed
            };

            _contextMock.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order> { order });

            var command = new PayOrderCommand(orderId);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_OrderInCreatedState_SetsStatusToProcessing()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Created 
            };

            _contextMock.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order> { order });

            var command = new PayOrderCommand(orderId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _contextMock.Verify(x => x.Orders.Update(It.Is<Order>(o => o.Status == OrderStatus.Processing)));
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
