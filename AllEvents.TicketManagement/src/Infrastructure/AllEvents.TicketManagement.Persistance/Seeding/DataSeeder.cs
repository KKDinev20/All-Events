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

        public DataSeeder(ReadEventsServiceReader readEventsServiceReader)
        {
            this.readEventsServiceReader = readEventsServiceReader;
        }

        public async Task SeedAsync(string filePath)
        {
            var events = await readEventsServiceReader.ReadAndSeedDataFromExcel(filePath);
        }
    }
}
