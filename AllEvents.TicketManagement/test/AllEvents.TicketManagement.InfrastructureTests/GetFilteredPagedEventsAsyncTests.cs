using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.InfrastructureTests
{
    public class GetFilteredPagedEventsAsyncTests
    {
        private async Task<AllEventsDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AllEventsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AllEventsDbContext(options);
            context.Database.EnsureCreated();

            context.Events.AddRange(new List<Event>
        {
            new Event { EventId = Guid.NewGuid(), Title = "Event 1", Location ="A", Price = 10, EventDate = DateTime.Now.AddDays(1), Category = EventCategory.Quiz },
            new Event { EventId = Guid.NewGuid(), Title = "Event 2", Location ="A", Price = 20, EventDate = DateTime.Now.AddDays(2), Category = EventCategory.Other },
            new Event { EventId = Guid.NewGuid(), Title = "Event 3", Location ="A", Price = 30, EventDate = DateTime.Now.AddDays(3), Category = EventCategory.Music },
        });

            await context.SaveChangesAsync();
            return context;
        }

        [Fact]
        public async Task GetFilteredPagedEventsAsync_ShouldFilterByTitle()
        {
            var context = await GetDbContext();
            var repository = new EventRepository(context);

            var events = await repository.GetFilteredPagedEventsAsync(0, 10, "Event 1", null, null, true);

            Assert.Single(events);
            Assert.Equal("Event 1", events.First().Title);
        }

        [Fact]
        public async Task GetFilteredPagedEventsAsync_ShouldFilterByCategory()
        {
            var context = await GetDbContext();
            var repository = new EventRepository(context);

            var events = await repository.GetFilteredPagedEventsAsync(0, 10, null, EventCategory.Other, null, true);

            Assert.Single(events);
            Assert.Equal(EventCategory.Other, events.First().Category);
        }

        [Fact]
        public async Task GetFilteredPagedEventsAsync_ShouldSortByDate()
        {
            var context = await GetDbContext();
            var repository = new EventRepository(context);

            var events = await repository.GetFilteredPagedEventsAsync(0, 10, null, null, "EventDate", true);

            Assert.Equal(3, events.Count);
            Assert.True(events[0].EventDate < events[1].EventDate);
            Assert.True(events[1].EventDate < events[2].EventDate);
        }

        [Fact]
        public async Task GetFilteredPagedEventsAsync_ShouldSortByPrice()
        {
            var context = await GetDbContext();
            var repository = new EventRepository(context);

            var events = await repository.GetFilteredPagedEventsAsync(0, 10, null, null, "Price", true);

            Assert.Equal(3, events.Count);
            Assert.True(events[0].Price < events[1].Price);
            Assert.True(events[1].Price < events[2].Price);
        }
    }
}
