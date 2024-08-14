using AllEvents.TicketManagement.Persistance;
using AllEvents.TicketManagement.Persistance.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;

namespace AllEvents.TicketManagement.InfrastructureTests
{
    public class QueryHandlerInterceptorTests
    {
        private readonly Mock<ILogger<QueryHandlerInterceptor>> _loggerMock;
        private readonly DbContextOptions<AllEventsDbContext> _dbContextOptions;
        private readonly TimeSpan _threshold = TimeSpan.FromMilliseconds(500);

        public QueryHandlerInterceptorTests()
        {
            _loggerMock = new Mock<ILogger<QueryHandlerInterceptor>>();

            _dbContextOptions = new DbContextOptionsBuilder<AllEventsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .AddInterceptors(new QueryHandlerInterceptor(_loggerMock.Object, _threshold))
                .Options;
        }

        private AllEventsDbContext CreateDbContext() => new AllEventsDbContext(_dbContextOptions, new Mock<ILoggerFactory>().Object);

        [Fact]
        public async Task Should_LogWarning_For_SlowQuery()
        {
            // Arrange
            using var context = CreateDbContext();

            // Act
            var stopwatch = Stopwatch.StartNew();
            await Task.Delay(_threshold + TimeSpan.FromMilliseconds(100));
            await context.Events.FirstOrDefaultAsync();
            stopwatch.Stop();

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Slow Query Detected")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Should_Not_Log_For_FastQuery()
        {
            // Arrange
            using var context = CreateDbContext();

            // Act
            // Run a query that should be considered fast
            await context.Events.FirstOrDefaultAsync();

            // Assert that no warning logs were created
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }
    }
}
