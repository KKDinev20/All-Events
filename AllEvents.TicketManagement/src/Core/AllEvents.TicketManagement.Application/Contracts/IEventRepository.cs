using AllEvents.TicketManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Application.Contracts
{
    public interface IEventRepository
    {
        Task<int> GetCountAsync();
        Task<List<Event>> GetPagedEventsAsync(int page, int pageSize);
    }
}
