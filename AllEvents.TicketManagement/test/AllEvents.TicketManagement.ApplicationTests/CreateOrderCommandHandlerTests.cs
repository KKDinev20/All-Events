using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.ExternalUsers.Commands;
using AllEvents.TicketManagement.Application.Features.Orders.Commands;
using AllEvents.TicketManagement.Application.Features.Orders.Handlers;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using MockQueryable.Moq;
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
        public async Task CreateOrderCommandHandler_SuccessfullyCreatesOrder()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new CreateOrderCommand
            {
                ExternalUserEmail = "test@example.com",
                EventId = eventId,
                TicketNames = new List<string> { "Ticket1", "Ticket2" }
            };

            var eventEntity = new Event
            {
                EventId = eventId,
                NrOfTickets = 10,
                Price = 100
            };

            var externalUser = new ExternalUser
            {
                Id = userId,
                Email = "test@example.com",
                Orders = new List<Order>()
            };

            var orders = new List<Order>().AsQueryable();
            var events = new List<Event> { eventEntity }.AsQueryable();
            var externalUsers = new List<ExternalUser> { externalUser }.AsQueryable();

            _contextMock.Setup(x => x.Events)
                .Returns(events.BuildMockDbSet().Object);

            _contextMock.Setup(x => x.Orders)
                .Returns(orders.BuildMockDbSet().Object);

            _contextMock.Setup(x => x.ExternalUsers)
                .Returns(externalUsers.BuildMockDbSet().Object);

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateExternalUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(externalUser);

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
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Cannot order more than 8 tickets per event.", exception.Message);
        }

        [Fact]
        public async Task CreateOrderCommandHandler_ThrowsException_WhenNotEnoughTickets()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                ExternalUserEmail = "test@example.com",
                EventId = Guid.NewGuid(),
                TicketNames = new List<string> { "Ticket1", "Ticket2" }
            };

            var eventEntity = new Event
            {
                EventId = command.EventId,
                NrOfTickets = 1,
                Price = 100
            };

            _contextMock.Setup(x => x.Events)
                .ReturnsDbSet(new List<Event> { eventEntity });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Not enough tickets available or event does not exist.", exception.Message);
        }

        [Fact]
        public async Task CreateOrderCommandHandler_ApplyCouponDiscount_CorrectlyAppliesDiscount()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var command = new CreateOrderCommand
            {
                ExternalUserEmail = "test@example.com",
                EventId = eventId,
                PromoCode = "Code",
                TicketNames = new List<string> { "Ticket1", "Ticket2" }
            };

            var eventEntity = new Event
            {
                EventId = eventId,
                NrOfTickets = 10,
                Price = 100,
                Coupons = new List<Coupon>
                {
                    new Coupon
                    {
                        Name = "Code",
                        FixedDiscountAmount = 20,
                        FromDate = DateTime.Now.AddDays(-1),
                        ToDate = DateTime.Now.AddDays(1)
                    }
                }
             };

            var events = new List<Event> { eventEntity }.AsQueryable();
            var coupons = eventEntity.Coupons.AsQueryable();

            _contextMock.Setup(x => x.Events)
                .Returns(events.BuildMockDbSet().Object);

            _contextMock.Setup(x => x.Coupons)
                .Returns(coupons.BuildMockDbSet().Object);

            // Act
            var totalAmount = await _handler.ApplyCouponDiscount(command, eventEntity, CancellationToken.None);

            // Assert
            Assert.Equal(180, totalAmount); 
        }


        [Fact]
        public async Task CreateOrderCommandHandler_ApplyLoyaltyDiscount_CorrectlyAppliesDiscount()
        {
            // Arrange
            decimal totalAmountSpent = 1200m;

            // Act
            var loyaltyDiscount = CreateOrderCommandHandler.ApplyLoyaltyDiscount(totalAmountSpent);

            // Assert
            Assert.Equal(0.05m, loyaltyDiscount);
        }

        [Fact]
        public async Task CreateOrderCommandHandler_CouponNotFound_DoesNotApplyDiscount()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var command = new CreateOrderCommand
            {
                ExternalUserEmail = "test@example.com",
                EventId = eventId,
                PromoCode = "NonExistentCode",
                TicketNames = new List<string> { "Ticket1", "Ticket2" }
            };

            var eventEntity = new Event
            {
                EventId = eventId,
                NrOfTickets = 10,
                Price = 100
            };

            var events = new List<Event> { eventEntity }.AsQueryable();
            _contextMock.Setup(x => x.Events)
                .Returns(events.BuildMockDbSet().Object);

            var coupons = new List<Coupon>().AsQueryable();
            _contextMock.Setup(x => x.Coupons)
                .Returns(coupons.BuildMockDbSet().Object);

            // Act
            var totalAmount = await _handler.ApplyCouponDiscount(command, eventEntity, CancellationToken.None);

            // Assert
            Assert.Equal(200, totalAmount);
        }

    }

}
