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
            var command = new PayOrderCommand(Guid.NewGuid(), "PromoCode");

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

            var command = new PayOrderCommand(orderId, "PromoCode");

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
                Status = OrderStatus.Created,
                TotalPrice = 100.0m
            };

            _contextMock.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order> { order });

            var command = new PayOrderCommand(orderId, null); 
            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(OrderStatus.Processing, order.Status);
            _contextMock.Verify(x => x.Orders.Update(It.Is<Order>(o => o.Status == OrderStatus.Processing)));
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentPromoCode_ThrowsInvalidOperationException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Created,
                TotalPrice = 100.0m
            };

            _contextMock.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order> { order });

            _contextMock.Setup(x => x.Coupons)
                .ReturnsDbSet(new List<Coupon>()); 

            var command = new PayOrderCommand(orderId, "INVALIDCODE");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Invalid or expired promo code.", exception.Message);

            Assert.Equal(100.0m, order.TotalPrice);
            _contextMock.Verify(x => x.Orders.Update(It.IsAny<Order>()), Times.Never);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithFivePercentDiscountCoupon_AppliesPercentageDiscount()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Created,
                TotalPrice = 100.0m
            };

            var coupon = new Coupon
            {
                Name = "5PERCENT",
                DiscountPercent = 5,
                FixedDiscountAmount = null,
                FromDate = new DateTime(2000, 1, 1),
                ToDate = new DateTime(2025, 12, 31)
            };

            _contextMock.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order> { order });

            _contextMock.Setup(x => x.Coupons)
                .ReturnsDbSet(new List<Coupon> { coupon });

            var command = new PayOrderCommand(orderId, "5PERCENT");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(95.0m, order.TotalPrice);
            Assert.Equal(OrderStatus.Processing, order.Status);
            _contextMock.Verify(x => x.Orders.Update(It.Is<Order>(o => o.Status == OrderStatus.Processing)));
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithTenUnitFixedDiscountCoupon_AppliesFixedAmountDiscount()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Created,
                TotalPrice = 100.0m
            };

            var coupon = new Coupon
            {
                Name = "10UNITOFF",
                DiscountPercent = null,
                FixedDiscountAmount = 10.0m,
                FromDate = new DateTime(2000, 1, 1),
                ToDate = new DateTime(2025, 12, 31)
            };

            _contextMock.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order> { order });

            _contextMock.Setup(x => x.Coupons)
                .ReturnsDbSet(new List<Coupon> { coupon });

            var command = new PayOrderCommand(orderId, "10UNITOFF");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(90.0m, order.TotalPrice);
            Assert.Equal(OrderStatus.Processing, order.Status);
            _contextMock.Verify(x => x.Orders.Update(It.Is<Order>(o => o.Status == OrderStatus.Processing)));
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


    }
}
