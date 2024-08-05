using AllEvents.TicketManagement.Application.Contracts;
using AllEvents.TicketManagement.Application.Features.Tickets.Commands;
using AllEvents.TicketManagement.Application.Features.Tickets.Handlers;
using AllEvents.TicketManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Cryptography;
using System.Text;

namespace AllEvents.TicketManagement.ApplicationTests
{
    public class ValidateTicketCommandHandlerTests
    {
        private byte[] EncryptData(string plainText, byte[] aesKey, byte[] aesIV)
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

        [Fact]
        public async Task ValidateTicketCommand_Should_Fail_For_Invalid_Name()
        {
            // Arrange
            var mockTicketRepository = new Mock<ITicketRepository>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Security:AES_Key", "AllEvents2024891" },
                { "Security:AES_IV", "E1F5D1A2C9B81234" }
            }).Build();

            var aesKey = Encoding.UTF8.GetBytes(configuration["Security:AES_Key"]);
            var aesIV = Encoding.UTF8.GetBytes(configuration["Security:AES_IV"]);

            var validTicketId = Guid.NewGuid();
            var validPersonName = "Valid Person";
            var invalidPersonName = "Invalid Person";

            var encryptedData = EncryptData($"{validTicketId}:{invalidPersonName}", aesKey, aesIV);
            var token = Convert.ToBase64String(encryptedData);

            var ticket = new Ticket(validTicketId, validPersonName, "Event Title", new byte[0], Guid.NewGuid());

            mockTicketRepository.Setup(repo => repo.GetByIdAsync(validTicketId)).ReturnsAsync(ticket);

            var handler = new ValidateTicketCommandHandler(configuration, mockTicketRepository.Object);

            // Act
            var result = await handler.Handle(new ValidateTicketCommand { Token = token }, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Validation Failed", result.Message);
        }


        [Fact]
        public async Task ValidateTicketCommand_Should_Fail_For_Invalid_GUID()
        {
            // Arrange
            var mockTicketRepository = new Mock<ITicketRepository>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Security:AES_Key", "AllEvents2024891" },
                { "Security:AES_IV", "E1F5D1A2C9B81234" }
            }).Build();

            var aesKey = Encoding.UTF8.GetBytes(configuration["Security:AES_Key"]);
            var aesIV = Encoding.UTF8.GetBytes(configuration["Security:AES_IV"]);

            var invalidTicketId = Guid.NewGuid();
            var validPersonName = "Valid Person";

            var encryptedData = EncryptData($"{invalidTicketId}:{validPersonName}", aesKey, aesIV);
            var token = Convert.ToBase64String(encryptedData);

            mockTicketRepository.Setup(repo => repo.GetByIdAsync(invalidTicketId)).ReturnsAsync((Ticket)null);

            var handler = new ValidateTicketCommandHandler(configuration, mockTicketRepository.Object);

            // Act
            var result = await handler.Handle(new ValidateTicketCommand { Token = token }, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Validation Failed", result.Message);
        }

        [Fact]
        public async Task ValidateTicketCommand_Should_Return_False_If_Ticket_Is_Null()
        {
            // Arrange
            var mockTicketRepository = new Mock<ITicketRepository>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Security:AES_Key", "AllEvents2024891" },
                { "Security:AES_IV", "E1F5D1A2C9B81234" }
            }).Build();

            var aesKey = Encoding.UTF8.GetBytes(configuration["Security:AES_Key"]);
            var aesIV = Encoding.UTF8.GetBytes(configuration["Security:AES_IV"]);

            var validTicketId = Guid.NewGuid();
            var validPersonName = "Valid Person";

            var encryptedData = EncryptData($"{validTicketId}:{validPersonName}", aesKey, aesIV);
            var token = Convert.ToBase64String(encryptedData);

            mockTicketRepository.Setup(repo => repo.GetByIdAsync(validTicketId)).ReturnsAsync((Ticket)null);

            var handler = new ValidateTicketCommandHandler(configuration, mockTicketRepository.Object);

            // Act
            var result = await handler.Handle(new ValidateTicketCommand { Token = token }, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Validation Failed", result.Message);
        }

        [Fact]
        public async Task ValidateTicketCommand_Should_Return_False_For_Invalid_Token()
        {
            // Arrange
            var mockTicketRepository = new Mock<ITicketRepository>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Security:AES_Key", "AllEvents2024891" },
                { "Security:AES_IV", "E1F5D1A2C9B81234" }
            }).Build(); 

            var handler = new ValidateTicketCommandHandler(configuration, mockTicketRepository.Object);

            // Act
            var result = await handler.Handle(new ValidateTicketCommand { Token = "invalid-token" }, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Invalid Token", result.Message);
        }

        [Fact]
        public async Task ValidateTicketCommand_Should_Return_True_For_Valid_Token()
        {
            var mockTicketRepository = new Mock<ITicketRepository>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Security:AES_Key", "AllEvents2024891" },
                { "Security:AES_IV", "E1F5D1A2C9B81234" }
            }).Build();

            var aesKey = Encoding.UTF8.GetBytes(configuration["Security:AES_Key"]);
            var aesIV = Encoding.UTF8.GetBytes(configuration["Security:AES_IV"]);

            var validTicketId = Guid.NewGuid();
            var validPersonName = "Valid Person";

            var encryptedData = EncryptData($"{validTicketId}:{validPersonName}", aesKey, aesIV);
            var token = Convert.ToBase64String(encryptedData);

            var ticket = new Ticket(validTicketId, validPersonName, "Event Title", new byte[0], Guid.NewGuid());

            mockTicketRepository.Setup(repo => repo.GetByIdAsync(validTicketId)).ReturnsAsync(ticket);

            var handler = new ValidateTicketCommandHandler(configuration, mockTicketRepository.Object);

            // Act
            var result = await handler.Handle(new ValidateTicketCommand { Token = token }, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Equal("Validation Success", result.Message);
        }

    }
}
