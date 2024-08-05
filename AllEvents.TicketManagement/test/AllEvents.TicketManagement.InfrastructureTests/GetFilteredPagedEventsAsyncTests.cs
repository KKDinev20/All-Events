using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Handlers;
using AllEvents.TicketManagement.Application.Features.Events.Queries;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.EntityFrameworkCore;
using Moq;

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
                new Event { EventId = Guid.NewGuid(), Title = "Event 1", Location = "A", Price = 10, EventDate = DateTime.Now.AddDays(1), Category = EventCategory.Quiz },
                new Event { EventId = Guid.NewGuid(), Title = "Event 2", Location = "A", Price = 20, EventDate = DateTime.Now.AddDays(2), Category = EventCategory.Other },
                new Event { EventId = Guid.NewGuid(), Title = "Event 3", Location = "A", Price = 30, EventDate = DateTime.Now.AddDays(3), Category = EventCategory.Music },
            });

            await context.SaveChangesAsync();
            return context;
        }
        private IEventQuery MockEventQuery(AllEventsDbContext context)
        {
            var eventQueryMock = new Mock<IEventQuery>();

            eventQueryMock.Setup(q => q.ExcludeDeleted()).Returns(eventQueryMock.Object);
            eventQueryMock.Setup(q => q.Search(It.IsAny<string>())).Returns((string title) =>
            {
                eventQueryMock.Setup(q => q.ToListAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(
                    context.Events.Where(e => e.Title.Contains(title)).ToList());
                return eventQueryMock.Object;
            });
            eventQueryMock.Setup(q => q.ForCategory(It.IsAny<EventCategory>())).Returns((EventCategory category) =>
            {
                eventQueryMock.Setup(q => q.ToListAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(
                    context.Events.Where(e => e.Category == category).ToList());
                return eventQueryMock.Object;
            });
            eventQueryMock.Setup(q => q.SortBy(It.IsAny<string>(), It.IsAny<bool>())).Returns((string sortBy, bool ascending) =>
            {
                eventQueryMock.Setup(q => q.ToListAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(
                    context.Events.OrderBy(e => EF.Property<object>(e, sortBy)).ToList());
                return eventQueryMock.Object;
            });
            eventQueryMock.Setup(q => q.ToListAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(context.Events.ToList());
            eventQueryMock.Setup(q => q.CountAsync()).ReturnsAsync(context.Events.Count());

            return eventQueryMock.Object;
        }

        [Fact]
        public async Task GetFilteredPagedEventsAsyncShouldReturnExpectedResults_WhenValidationOK()
        {
            // Arrange
            var context = await GetDbContext();
            var eventQuery = MockEventQuery(context);
            var handler = new GetAllEventsQueryHandler(eventQuery);

            // Act
            var query = new GetAllEventsQuery(1, 10, null, null, null, true);
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(3, result.Items.Count);
            Assert.Contains(result.Items, e => e.Title == "Event 1");
            Assert.Contains(result.Items, e => e.Title == "Event 2");
            Assert.Contains(result.Items, e => e.Title == "Event 3");
        }

        [Fact]
        public async Task GetFilteredPagedEventsAsyncShouldFilterByTitle()
        {
            var context = await GetDbContext();
            var eventQuery = MockEventQuery(context);
            var handler = new GetAllEventsQueryHandler(eventQuery);

            var query = new GetAllEventsQuery(1, 10, "Event 1", null, null, true);
            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Single(result.Items);
            Assert.Equal("Event 1", result.Items.First().Title);
        }

        [Fact]
        public async Task GetFilteredPagedEventsAsyncShouldFilterByCategory()
        {
            var context = await GetDbContext();
            var eventQuery = MockEventQuery(context);
            var handler = new GetAllEventsQueryHandler(eventQuery);

            var query = new GetAllEventsQuery(1, 10, null, EventCategory.Other, null, true);
            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Single(result.Items);
            Assert.Equal(EventCategory.Other, result.Items.First().Category);
        }

        [Fact]
        public async Task GetFilteredPagedEventsAsyncShouldCheckForFalseStatements()
        {
            var context = await GetDbContext();
            var eventQuery = MockEventQuery(context);
            var handler = new GetAllEventsQueryHandler(eventQuery);

            var query = new GetAllEventsQuery(1, 10, null, null, "EventDate", true);
            var result = await handler.Handle(query, CancellationToken.None);

            Assert.False(result.Items[0].EventDate > result.Items[1].EventDate);
            Assert.False(result.Items[1].EventDate > result.Items[2].EventDate);
        }

        [Fact]
        public async Task GetFilteredPagedEventsAsyncShouldSortByPrice()
        {
            var context = await GetDbContext();
            var eventQuery = MockEventQuery(context);
            var handler = new GetAllEventsQueryHandler(eventQuery);

            var query = new GetAllEventsQuery(1, 10, null, null, "Price", true);
            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(3, result.Items.Count);
            Assert.True(result.Items[0].Price < result.Items[1].Price);
            Assert.True(result.Items[1].Price < result.Items[2].Price);
        }
    }
}
