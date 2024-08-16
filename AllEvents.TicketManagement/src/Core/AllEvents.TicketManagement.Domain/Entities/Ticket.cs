namespace AllEvents.TicketManagement.Domain.Entities
{
    public class Ticket
    {
        public Guid TicketId { get; set; }
        public string PersonName { get; set; } = null!;
        public string EventTitle { get; set; } = null!;
        public byte[] QRCode { get; set; } = null!;
        public Guid EventId { get; set; }
        public Event Event { get; set; } = null!;

        public Ticket(Guid ticketId, string personName, string eventTitle, byte[] qRCode, Guid eventId)
        {
            TicketId = ticketId;
            PersonName = personName;
            EventTitle = eventTitle;
            QRCode = qRCode;
            EventId = eventId;
        }
    }
}
