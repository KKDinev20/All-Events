using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Events.Commands.DeleteEvent;
using AllEvents.TicketManagement.Application.Features.Events.Handlers;
using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class DeleteEventsCommandHandlerTests
{
    private readonly IMediator _mediator;
    private readonly IAllEventsDbContext _context;

    public DeleteEventsCommandHandlerTests()
    {
        var services = new ServiceCollection();
        services.AddDbContext<AllEventsDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: "AllEvents"));

        services.AddScoped<IAllEventsDbContext>(provider => provider.GetService<AllEventsDbContext>());
        services.AddMediatR(typeof(DeleteEventCommandHandler).Assembly);

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
        var eventId = _context.Events.First().EventId;

        var command = new DeleteEventCommand { EventId = eventId };
        var result = await _mediator.Send(command);


        var deletedEvent = await _context.Events.FindAsync(eventId);
        Assert.NotNull(deletedEvent);
        Assert.True(deletedEvent.IsDeleted);
    }

    [Fact]
    public async Task DeleteEventCommand_Should_Not_Affect_Other_Events()
    {
        var eventId = _context.Events.First().EventId;
        var anotherEventId = _context.Events.Skip(1).First().EventId;

        var command = new DeleteEventCommand { EventId = eventId };
        await _mediator.Send(command);

        var notDeletedEvent = await _context.Events.FindAsync(anotherEventId);
        Assert.False(notDeletedEvent.IsDeleted);
    }

    [Fact]
    public async Task DeleteEventCommand_Should_Return_False_If_Event_Not_Found()
    {
        var nonExistentEventId = Guid.NewGuid();

        var command = new DeleteEventCommand { EventId = nonExistentEventId };
        var result = await _mediator.Send(command);
        Assert.False(result);
    }
}
