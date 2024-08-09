using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands.DeleteEvent;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllEvents.TicketManagement.InfrastructureTests
{
    public class DeleteEventsCommandHandlerTests
    {
        private readonly IMediator _mediator;
        private readonly IAllEventsDbContext _context;
        private readonly Mock<IDistributedCache> _mockCache;

        public DeleteEventsCommandHandlerTests()
        {
            var services = new ServiceCollection();

            var loggerFactory = new LoggerFactory();
            services.AddSingleton<ILoggerFactory>(loggerFactory);

            services.AddDbContext<AllEventsDbContext>((provider, options) =>
                options.UseInMemoryDatabase(databaseName: "AllEvents")
                       .UseLoggerFactory(loggerFactory));

            services.AddScoped<IAllEventsDbContext>(provider => provider.GetService<AllEventsDbContext>());
            services.AddMediatR(typeof(DeleteEventCommandHandler).Assembly);

            _mockCache = new Mock<IDistributedCache>();
            services.AddSingleton(_mockCache.Object);

            var serviceProvider = services.BuildServiceProvider();
            _context = serviceProvider.GetService<IAllEventsDbContext>();
            _mediator = serviceProvider.GetService<IMediator>();

            SeedDatabase().GetAwaiter().GetResult();
        }

        private async Task SeedDatabase()
        {
            var events = new[]
            {
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Test Event 1",
                    Location = "Test Location 1",
                    Price = 100,
                    Category = EventCategory.Other,
                    EventDate = DateTime.Now.AddMonths(1),
                    NrOfTickets = 100,
                    IsDeleted = false
                },
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Test Event 2",
                    Location = "Test Location 2",
                    Price = 200,
                    Category = EventCategory.Music,
                    EventDate = DateTime.Now.AddMonths(2),
                    NrOfTickets = 100,
                    IsDeleted = false
                }
            };

            _context.Events.AddRange(events);
            await _context.SaveChangesAsync(default);
        }

        [Fact]
        public async Task DeleteEventCommand_Should_Delete_Event()
        {
            var eventId = _context.Events.First(e => !e.IsDeleted).EventId;

            var command = new DeleteEventCommand { EventId = eventId };
            var result = await _mediator.Send(command);

            var deletedEvent = await _context.Events.FindAsync(eventId);
            Assert.True(result);
            Assert.True(deletedEvent.IsDeleted);
        }

        [Fact]
        public async Task DeleteEventCommand_Should_Return_False_If_Event_Not_Found()
        {
            var nonExistentEventId = Guid.NewGuid();

            var command = new DeleteEventCommand { EventId = nonExistentEventId };
            var result = await _mediator.Send(command);
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteEventCommand_Should_Return_False_If_Event_Already_Deleted()
        {
            var eventId = _context.Events.First(e => !e.IsDeleted).EventId;

            var command = new DeleteEventCommand { EventId = eventId };
            await _mediator.Send(command);

            var result = await _mediator.Send(command);
            Assert.False(result);
        }
    }
}
