using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AllEvents.TicketManagement.ApplicationTests
{
    public class EventQueryTests : IClassFixture<EventQueryTests>, IDisposable
    {
        private readonly DbContextOptions<AllEventsDbContext> _contextOptions;
        private readonly AllEventsDbContext _context;
        private readonly EventQuery _query;

        public EventQueryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<AllEventsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AllEventsDbContext(_contextOptions, LoggerFactory.Create(builder => builder.AddConsole()));
            SeedDatabase();

            _query = new EventQuery(_context.Events.AsQueryable());
        }

        private void SeedDatabase()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var events = new List<Event>
            {
                new Event { Title = "Music Concert", Location = "A", Price = 100, Category = EventCategory.Music },
                new Event { Title = "Tech Conference", Location = "B", Price = 50, Category = EventCategory.Other },
                new Event { Title = "Art Exhibition", Location = "C", Price = 70, Category = EventCategory.Quiz },
                new Event { Title = "Food Festival", Location = "D", Price = 90, Category = EventCategory.Festival }
            };

            _context.Events.AddRange(events);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Search_ShouldFilterByTitle()
        {
            // Act
            var result = await _query.Search("Concert").ToListAsync(0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal("Music Concert", result.First().Title);
        }

        [Fact]
        public async Task ForCategory_ShouldFilterByCategory()
        {
            // Act
            var result = await _query.ForCategory(EventCategory.Other).ToListAsync(0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal(EventCategory.Other, result.First().Category);
        }

        [Fact]
        public async Task SortBy_ShouldSortByTitleAscending()
        {
            // Act
            var result = await _query.SortBy("Title", true).ToListAsync(0, 10);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal("Art Exhibition", result.First().Title);
        }

        [Fact]
        public async Task SortBy_ShouldSortByTitleDescending()
        {
            // Act
            var result = await _query.SortBy("Title", false).ToListAsync(0, 10);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal("Tech Conference", result.First().Title);
        }

        [Fact]
        public async Task ToListAsync_ShouldReturnPagedResults()
        {
            // Act
            var result = await _query.ToListAsync(1, 2);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Art Exhibition", result.First().Title);
        }

        [Fact]
        public async Task CountAsync_ShouldReturnTotalCount()
        {
            // Act
            var count = await _query.CountAsync();

            // Assert
            Assert.Equal(4, count);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
