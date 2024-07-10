using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllEvents.TicketManagement.Application.Models
{
    public class TicketModel
    {
        public Guid TicketId { get; set; }
        public string PersonName { get; set; } = null!;
        public string EventTitle { get; set; } = null!;

        public byte[] QRCode { get; set; }
    }
}
