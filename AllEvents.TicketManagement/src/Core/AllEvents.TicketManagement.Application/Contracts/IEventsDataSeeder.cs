using AllEvents.TicketManagement.Domain.Entities;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IEventsDataSeeder
    {
       Task<List<Event>> ReadDataFromExcel(string filePath);
    }
}
