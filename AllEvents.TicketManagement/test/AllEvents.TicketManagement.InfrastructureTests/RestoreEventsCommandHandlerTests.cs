using AllEvents.TicketManagement.Domain.Entities;
using AllEvents.TicketManagement.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.InfrastructureTests
{
    public class RestoreEventsCommandHandlerTests
    {
        private readonly AllEventsDbContext _dbContext;
        private readonly EventRepository _eventRepository;

        public RestoreEventsCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AllEventsDbContext>()
                .UseInMemoryDatabase(databaseName: "AllEvents")
                .Options;
            _dbContext = new AllEventsDbContext(options);
            _eventRepository = new EventRepository(_dbContext);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _dbContext.Events.AddRange(
                new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = "Test Event 1",
                    Location = "Test Location 1",
                    Price = 100,
                    Category = EventCategory.Festival,
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
                    Category = EventCategory.Quiz,
                    EventDate = DateTime.Now.AddMonths(2),
                    NrOfTickets = 100,
                    IsDeleted = false
                }
            );
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task RestoreAsync_Should_Set_IsDeleted_To_False()
        {
            var eventId = _dbContext.Events.First(e => e.IsDeleted).EventId;

            await _eventRepository.RestoreAsync(eventId);

            var restoredEvent = await _dbContext.Events.FindAsync(eventId);
            Assert.False(restoredEvent.IsDeleted);
        }

        [Fact]
        public async Task RestoreAsync_Should_Throw_Exception_If_Event_Not_Found()
        {
            var nonExistentEventId = Guid.NewGuid();

            await Assert.ThrowsAsync<InvalidOperationException>(() => _eventRepository.RestoreAsync(nonExistentEventId));
        }
    }
}
