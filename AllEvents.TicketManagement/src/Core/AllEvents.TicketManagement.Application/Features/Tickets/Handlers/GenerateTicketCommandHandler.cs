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
        private readonly IOrderRepository _orderRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly byte[] aesKey;
        private readonly byte[] aesIV;

        public GenerateTicketCommandHandler(IConfiguration configuration, IOrderRepository orderRepository, ITicketRepository ticketRepository, IEventRepository eventRepository)
        {
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));


            aesKey = Encoding.UTF8.GetBytes(configuration["Security:AES_Key"]);
            aesIV = Encoding.UTF8.GetBytes(configuration["Security:AES_IV"]);

            if (aesKey == null || aesIV == null || aesKey.Length == 0 || aesIV.Length == 0)
            {
                throw new ApplicationException("AES_KEY or AES_IV configuration is missing or empty.");
            }
        }

        public async Task<TicketModel> Handle(GenerateTicketCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            CheckOrder(request, order);

            var @event = await _eventRepository.GetByIdAsync(request.EventId);
            if (@event == null)
            {
                throw new ArgumentException($"Event with ID {request.EventId} not found.");
            }

            Ticket ticket = await GenerateTicket(request, @event);
            await UpdateOrder(request, order);

            return new TicketModel(
                ticketId: ticket.TicketId,
                personName: ticket.PersonName,
                eventTitle: ticket.EventTitle,
                qRCode: ticket.QRCode
            );
        }

        private async Task UpdateOrder(GenerateTicketCommand request, Order? order)
        {
            order.TicketNames.Remove(request.PersonName);

            if (order.TicketNames.Count == 0)
            {
                order.Status = OrderStatus.Completed;
            }

            await _orderRepository.UpdateAsync(order);
        }

        private async Task<Ticket> GenerateTicket(GenerateTicketCommand request, Event? @event)
        {
            var ticketId = Guid.NewGuid();
            var encryptedData = EncryptData($"{ticketId}:{request.PersonName}");
            var appBaseUrl = "https://localhost:7273";
            var qrCodeUrl = $"{appBaseUrl}/api/ticket/validate?token={Convert.ToBase64String(encryptedData)}";
            var qrCodeImage = GenerateQRCodeImage(qrCodeUrl);

            var ticket = new Ticket(
                ticketId: ticketId,
                personName: request.PersonName,
                eventTitle: @event.Title,
                qRCode: qrCodeImage,
                eventId: @event.EventId
            );

            await _ticketRepository.AddAsync(ticket);
            return ticket;
        }

        private static void CheckOrder(GenerateTicketCommand request, Order? order)
        {
            if (order == null)
            {
                throw new ArgumentException($"Order with ID {request.OrderId} not found.");
            }

            if (order.Status != OrderStatus.Processing)
            {
                throw new InvalidOperationException("Tickets can only be generated for orders in the 'Processing' state.");
            }

            if (!order.TicketNames.Contains(request.PersonName))
            {
                throw new ArgumentException("The specified name does not match any names in the order.");
            }
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

        private byte[] GenerateQRCodeImage(string qrCodeUrl)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
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
