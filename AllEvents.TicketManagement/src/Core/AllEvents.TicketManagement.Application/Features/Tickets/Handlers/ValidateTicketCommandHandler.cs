using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Tickets.Commands;
using AllEvents.TicketManagement.Application.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace AllEvents.TicketManagement.Application.Features.Tickets.Handlers
{
    public class ValidateTicketCommandHandler : IRequestHandler<ValidateTicketCommand, ValidationResult>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly byte[] aesKey;
        private readonly byte[] aesIV;

        public ValidateTicketCommandHandler(IConfiguration configuration, ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
            aesKey = Encoding.UTF8.GetBytes(configuration["Security:AES_Key"]);
            aesIV = Encoding.UTF8.GetBytes(configuration["Security:AES_IV"]);
        }

        public async Task<ValidationResult> Handle(ValidateTicketCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var decryptedData = DecryptData(Convert.FromBase64String(request.Token));
                var parts = decryptedData.Split(':');
                var ticketId = Guid.Parse(parts[0]);
                var personName = parts[1];

                var ticket = await _ticketRepository.GetByIdAsync(ticketId);
                if (ticket != null && ticket.PersonName == personName)
                {
                    return new ValidationResult { IsSuccessful = true, Message = "Validation Success" };
                }
                return new ValidationResult { IsSuccessful = false, Message = "Validation Failed" };
            }
            catch
            {
                return new ValidationResult { IsSuccessful = false, Message = "Invalid Token" };
            }
        }

        private string DecryptData(byte[] cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = aesIV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(cipherText))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
