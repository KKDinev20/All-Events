using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.ExternalUsers.Commands;
using AllEvents.TicketManagement.Application.Features.ExternalUsers.Handlers;
using AllEvents.TicketManagement.Domain.Entities;
using Moq;
using Moq.EntityFrameworkCore;

namespace AllEvents.TicketManagement.Application.UnitTests.ExternalUsers
{
    public class CreateExternalUserCommandHandlerTests
    {
        private readonly Mock<IAllEventsDbContext> _contextMock;
        private readonly CreateExternalUserCommandHandler _handler;

        public CreateExternalUserCommandHandlerTests()
        {
            _contextMock = new Mock<IAllEventsDbContext>();
            _handler = new CreateExternalUserCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task CreateExternalUserCommandHandler_ShouldCreateNewExternalUser_WhenUserDoesNotExist()
        {
            // Arrange
            var command = new CreateExternalUserCommand("newuser@example.com");

            _contextMock.Setup(x => x.ExternalUsers)
                .ReturnsDbSet(new List<ExternalUser>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser@example.com", result.Email);
            _contextMock.Verify(x => x.ExternalUsers.Add(It.IsAny<ExternalUser>()), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateExternalUserCommandHandler_ShouldReturnExistingUser_WhenUserAlreadyExists()
        {
            // Arrange
            var existingUser = new ExternalUser { Id = Guid.NewGuid(), Email = "existinguser@example.com" };
            var command = new CreateExternalUserCommand(existingUser.Email);

            _contextMock.Setup(x => x.ExternalUsers)
                .ReturnsDbSet(new List<ExternalUser> { existingUser });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingUser.Id, result.Id);
            Assert.Equal(existingUser.Email, result.Email);
            _contextMock.Verify(x => x.ExternalUsers.Add(It.IsAny<ExternalUser>()), Times.Never);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
