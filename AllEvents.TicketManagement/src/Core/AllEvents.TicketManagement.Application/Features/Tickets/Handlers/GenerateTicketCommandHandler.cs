using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Tickets.Commands;
using AllEvents.TicketManagement.Application.Models;
using AllEvents.TicketManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
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
        private readonly byte[] aesKey;
        private readonly byte[] aesIV;

        public GenerateTicketCommandHandler(IConfiguration configuration, IEventRepository eventRepository, ITicketRepository ticketRepository)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));

            aesKey = Encoding.UTF8.GetBytes(configuration["Security:AES_Key"]);
            aesIV = Encoding.UTF8.GetBytes(configuration["Security:AES_IV"]);

            if (aesKey == null || aesIV == null || aesKey.Length == 0 || aesIV.Length == 0)
            {
                throw new ApplicationException("AES_KEY or AES_IV configuration is missing or empty.");
            }
        }


        public async Task<TicketModel> Handle(GenerateTicketCommand request, CancellationToken cancellationToken)
        {
            var @event = await _eventRepository.GetByIdAsync(request.EventId);

            if (@event == null)
            {
                throw new ArgumentException($"Event with ID {request.EventId} not found.");
            }

            var ticketId = Guid.NewGuid();
            var encryptedData = EncryptData($"{ticketId}:{request.PersonName}");
            var qrCodeImage = GenerateQRCodeImage(encryptedData);

            var ticket = new Ticket(
                ticketId: ticketId,
                personName: request.PersonName,
                eventTitle: @event.Title,
                qRCode: qrCodeImage,
                eventId: @event.EventId
            );

            await _ticketRepository.AddAsync(ticket);

            return new TicketModel(
                ticketId: ticket.TicketId,
                personName: ticket.PersonName,
                eventTitle: ticket.EventTitle,
                qRCode: ticket.QRCode
            );
        }

        private byte[] EncryptData(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = aesIV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        return ms.ToArray();
                    }
                }
            }
        }


        private byte[] GenerateQRCodeImage(byte[] encryptedData)
        {
            var encryptedString = Convert.ToBase64String(encryptedData);
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(encryptedString, QRCodeGenerator.ECCLevel.Q);
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
