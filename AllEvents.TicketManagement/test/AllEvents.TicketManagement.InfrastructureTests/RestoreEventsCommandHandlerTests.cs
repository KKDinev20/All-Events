using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands;
using AllEvents.TicketManagement.Application.Features.Events.Handlers;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AllEvents.TicketManagement.InfrastructureTests
{
    public class RestoreEventsCommandHandlerTests
    {
        private readonly IMediator _mediator;
        private readonly IAllEventsDbContext _context;

        public RestoreEventsCommandHandlerTests()
        {
            var services = new ServiceCollection();
            services.AddDbContext<AllEventsDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "AllEvents"));

            services.AddScoped<IAllEventsDbContext>(provider => provider.GetService<AllEventsDbContext>());
            services.AddMediatR(typeof(RestoreEventCommandHandler).Assembly);

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
                    IsDeleted = true
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
        public async Task RestoreEventCommand_Should_Restore_Event()
        {
            var eventId = _context.Events.First(e => e.IsDeleted).EventId;

            var command = new RestoreEventCommand { EventId = eventId };
            var result = await _mediator.Send(command);

            var restoredEvent = await _context.Events.FindAsync(eventId);
            Assert.True(result);
            Assert.False(restoredEvent.IsDeleted);
        }

        [Fact]
        public async Task RestoreEventCommand_Should_Return_False_If_Event_Not_Found()
        {
            var nonExistentEventId = Guid.NewGuid();

            var command = new RestoreEventCommand { EventId = nonExistentEventId };
            var result = await _mediator.Send(command);
            Assert.False(result);
        }
    }
}