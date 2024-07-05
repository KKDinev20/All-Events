using AllEvents.TicketManagement.Persistance.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Persistance.Seeding
{
    public class DataSeeder
    {
        private readonly ReadEventsServiceReader readEventsServiceReader;
        private readonly AllEventsDbContext allEventsDbContext;

        public DataSeeder(ReadEventsServiceReader readEventsServiceReader, AllEventsDbContext allEventsDbContext)
        {
            this.readEventsServiceReader = readEventsServiceReader;
            this.allEventsDbContext = allEventsDbContext;
        }

        public async Task SeedAsync(string filePath)
        {
            var events = await readEventsServiceReader.ReadAndSeedDataFromExcel(filePath);

            if (!allEventsDbContext.Events.Any())
            {
                allEventsDbContext.Events.AddRange(events);
                await allEventsDbContext.SaveChangesAsync();
            }
        }
    }
}
