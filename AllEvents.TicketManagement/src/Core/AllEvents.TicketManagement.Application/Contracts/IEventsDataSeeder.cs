using AllEvents.TicketManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IEventsDataSeeder
    {
       Task<List<Event>> ReadDataFromExcel(string filePath);
    }
}
