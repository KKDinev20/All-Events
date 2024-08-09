using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Extensions;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AllEvents.TicketManagement.ApplicationTests
{
    public class GetFilteredEventsQueryHandlerTests
    {
        private readonly IAllEventsDbContext _dbContext;
        private readonly IDistributedCache _cache;
        private readonly ILogger<GetFilteredEventsQueryHandler> _logger;
        private readonly GetFilteredEventsQueryHandler _handler;

        public GetFilteredEventsQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AllEventsDbContext>()
                .UseInMemoryDatabase(databaseName: "AllEvents")
                .Options;

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            _dbContext = new AllEventsDbContext(options, loggerFactory);
            var cacheOptions = new MemoryDistributedCacheOptions();
            _cache = new MemoryDistributedCache(Options.Create(cacheOptions));
            _logger = loggerFactory.CreateLogger<GetFilteredEventsQueryHandler>();

            SeedDatabase();

            _handler = new GetFilteredEventsQueryHandler(_dbContext, _cache, _logger);
        }

        private void SeedDatabase()
        {
            if (!_dbContext.Events.Any())
            {
                var events = new List<Event>
            {
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Music Event",
                    Location = "New York",
                    Price = 100.00m,
                    Category = EventCategory.Music,
                    EventDate = DateTime.Now.AddDays(10),
                    NrOfTickets = 100
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Sports Event",
                    Location = "Los Angeles",
                    Price = 150.00m,
                    Category = EventCategory.Sports,
                    EventDate = DateTime.Now.AddDays(20),
                    NrOfTickets = 200
                }
            };

                _dbContext.Events.AddRange(events);
                _dbContext.SaveChangesAsync(CancellationToken.None).Wait();
            }
        }

        [Fact]
        public async Task Handle_ShouldReturnCachedResult_WhenCacheHit()
        {
            // Arrange
            var query = new GetFilteredEventsQuery(0, 10, "Music Event", null, null, true);

            var cachedResult = new PagedResult<EventModel>(new List<EventModel>
        {
            new EventModel { EventId = Guid.NewGuid(), Title = "Cached Music Event", Location = "Cached Location", Price = 50.00m, Category = EventCategory.Music }
        }, 1, 1, 10);

            string cacheKey = "Event_0_10_Music Event__null_True";
            await _cache.SetCacheAsync(cacheKey, cachedResult, new DistributedCacheEntryOptions(), "Event");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Music Event", result.Items.First().Title);
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredEvents()
        {
            // Arrange
            var query = new GetFilteredEventsQuery(0, 10, "Music Event", null, null, true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Music Event", result.Items.First().Title);
        }

        [Fact]
        public async Task Handle_ShouldReturnSortedEvents()
        {
            // Arrange
            var query = new GetFilteredEventsQuery(0, 10, null, null, "Price", true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("Music Event", result.Items.First().Title);
            Assert.Equal("Sports Event", result.Items.Last().Title);
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedResults()
        {
            // Arrange
            var query = new GetFilteredEventsQuery(0, 1, null, null, null, true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(2, result.TotalCount);
        }
    }

}