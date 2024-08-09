using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AllEvents.TicketManagement.APITests
{
    public class RetrieveAllEventsTests
    {
        private readonly DbContextOptions<AllEventsDbContext> _mockOptions;
        private readonly ILoggerFactory _loggerFactory;

        public RetrieveAllEventsTests()
        {
            _mockOptions = new DbContextOptionsBuilder<AllEventsDbContext>()
                .UseInMemoryDatabase(databaseName: "AllEvents")
                .Options;
            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            using (var context = new AllEventsDbContext(_mockOptions, _loggerFactory))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var events = new List<Event>
                {
                    new Event { Title = "Music Concert", Location = "A", Price = 100, Category = EventCategory.Music },
                    new Event { Title = "Tech Conference", Location = "B", Price = 50, Category = EventCategory.Other },
                    new Event { Title = "Art Exhibition", Location = "C", Price = 70, Category = EventCategory.Quiz },
                    new Event { Title = "Food Festival", Location = "D", Price = 90, Category = EventCategory.Festival }
                };

                context.Events.AddRange(events);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task RetrieveAllEvents_ShouldGiveEmptyPageWhenInvalidPageNumber()
        {
            // Arrange
            using (var context = new AllEventsDbContext(_mockOptions, _loggerFactory))
            {
                var repository = new EventRepository(context);
                int page = -2;
                int pageSize = 5;

                // Act
                var result = await repository.GetPagedEventsAsync(page, pageSize);

                // Assert
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task RetrieveAllEvents_ShouldGiveEmptyPageWhenInvalidPageSize()
        {
            // Arrange
            using (var context = new AllEventsDbContext(_mockOptions, _loggerFactory))
            {
                var repository = new EventRepository(context);
                int page = 2;
                int pageSize = -5;

                // Act
                var result = await repository.GetPagedEventsAsync(page, pageSize);

                // Assert
                Assert.Empty(result);
            }
        }
    }
}
