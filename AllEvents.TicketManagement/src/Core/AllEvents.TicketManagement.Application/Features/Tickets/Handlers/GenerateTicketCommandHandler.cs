using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Tickets.Commands;
using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using QRCoder;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;

namespace AllEvents.TicketManagement.Application.Features.Tickets.Handlers
{
    public class GenerateTicketCommandHandler : IRequestHandler<GenerateTicketCommand, TicketModel>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITicketRepository _ticketRepository;

        public GenerateTicketCommandHandler(IEventRepository eventRepository, ITicketRepository ticketRepository)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        }

        public async Task<TicketModel> Handle(GenerateTicketCommand request, CancellationToken cancellationToken)
        {
            var @event = await _eventRepository.GetByIdAsync(request.EventId);

            if (@event == null)
            {
                throw new ArgumentException($"Event with ID {request.EventId} not found.");
            }

            var ticketId = Guid.NewGuid();
            var qrCodeHash = GenerateQRCodeHash(ticketId, request.PersonName);
            var qrCodeImage = GenerateQRCodeImage(qrCodeHash);

            var ticket = new Ticket
            {
                TicketId = ticketId,
                PersonName = request.PersonName,
                EventTitle = @event.Title,
                QRCode = qrCodeImage,
                EventId = @event.EventId,
                Event = @event
            };

            await _ticketRepository.AddAsync(ticket);

            return new TicketModel
            {
                TicketId = ticket.TicketId,
                PersonName = ticket.PersonName,
                EventTitle = ticket.EventTitle,
                QRCode = ticket.QRCode
            };
        }

        private byte[] GenerateQRCodeHash(Guid ticketId, string personName)
        {
            var data = $"{ticketId}:{personName}";
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        private byte[] GenerateQRCodeImage(byte[] hash)
        {
            var hashString = BitConverter.ToString(hash).Replace("-", "");
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(hashString, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (var qrCodeImage = qrCode.GetGraphic(20))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            qrCodeImage.Save(memoryStream, ImageFormat.Png);
                            return memoryStream.ToArray();
                        }
                    }
                }
            }
        }
    }
}
