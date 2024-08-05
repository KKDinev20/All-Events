using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.ApplicationTests
{
    public class EventQueryTests
    {
        private DbContextOptions<AllEventsDbContext> _contextOptions;

        public EventQueryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<AllEventsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            using var context = new AllEventsDbContext(_contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var events = new List<Event>
        {
            new Event { Title = "Music Concert", Location = "A", Price=100, Category = EventCategory.Music },
            new Event { Title = "Tech Conference", Location = "B", Price=50, Category = EventCategory.Other },
            new Event {Title = "Art Exhibition", Location = "C", Price = 70, Category = EventCategory.Quiz},
            new Event {Title = "Food Festival", Location = "D", Price = 90, Category = EventCategory.Festival},
        };

            context.Events.AddRange(events);
            context.SaveChanges();
        }

        [Fact]
        public async Task Search_ShouldFilterByTitle()
        {
            // Arrange
            using var context = new AllEventsDbContext(_contextOptions);
            var query = new EventQuery(context.Events.AsQueryable());

            // Act
            var result = await query.Search("Concert").ToListAsync(0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal("Music Concert", result.First().Title);
        }

        [Fact]
        public async Task ForCategory_ShouldFilterByCategory()
        {
            // Arrange
            using var context = new AllEventsDbContext(_contextOptions);
            var query = new EventQuery(context.Events.AsQueryable());

            // Act
            var result = await query.ForCategory(EventCategory.Other).ToListAsync(0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal(EventCategory.Other, result.First().Category);
        }

        [Fact]
        public async Task SortBy_ShouldSortByTitleAscending()
        {
            // Arrange
            using var context = new AllEventsDbContext(_contextOptions);
            var query = new EventQuery(context.Events.AsQueryable());

            // Act
            var result = await query.SortBy("Title", true).ToListAsync(0, 10);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal("Art Exhibition", result.First().Title);
        }

        [Fact]
        public async Task SortBy_ShouldSortByTitleDescending()
        {
            // Arrange
            using var context = new AllEventsDbContext(_contextOptions);
            var query = new EventQuery(context.Events.AsQueryable());

            // Act
            var result = await query.SortBy("Title", false).ToListAsync(0, 10);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal("Tech Conference", result.First().Title);
        }

        [Fact]
        public async Task ToListAsync_ShouldReturnPagedResults()
        {
            // Arrange
            using var context = new AllEventsDbContext(_contextOptions);
            var query = new EventQuery(context.Events.AsQueryable());

            // Act
            var result = await query.ToListAsync(1, 2);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Art Exhibition", result.First().Title);
        }

        [Fact]
        public async Task CountAsync_ShouldReturnTotalCount()
        {
            // Arrange
            using var context = new AllEventsDbContext(_contextOptions);
            var query = new EventQuery(context.Events.AsQueryable());

            // Act
            var count = await query.CountAsync();

            // Assert
            Assert.Equal(4, count);
        }
    }

}
