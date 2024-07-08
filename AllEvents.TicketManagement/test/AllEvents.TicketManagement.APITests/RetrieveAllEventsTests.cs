using AllEvents.TicketManagement.Persistance;
using AllEvents.TicketManagement.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AllEvents.TicketManagement.APITests
{
    public class RetrieveAllEventsTests
    {
        [Fact]
        public async Task RetrieveAllEventsTestsShouldGiveEmptyPageWhenInvalidPageNumber()
        {
            //Assert
            var mockOptions = new DbContextOptionsBuilder<AllEventsDbContext>()
                .UseInMemoryDatabase(databaseName: "AllEvents")
                .Options;

            using (var context = new AllEventsDbContext(mockOptions))
            {
                var repository = new EventRepository(context);
                int page = 0;
                int pageSize = 10;

                // Act
                var result = await repository.GetPagedEventsAsync(page, pageSize);

                // Assert
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task RetrieveAllEventsTestsShouldGiveEmptyPageWhenInvalidPageSize()
        {
            //Assert
            var mockOptions = new DbContextOptionsBuilder<AllEventsDbContext>()
                .UseInMemoryDatabase(databaseName: "AllEvents")
                .Options;

            using (var context = new AllEventsDbContext(mockOptions))
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