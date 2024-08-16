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
    public class ValidateTicketCommandHandlerTests : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly Mock<ITicketRepository> _mockTicketRepository;
        private readonly ValidateTicketCommandHandler _handler;
        private readonly byte[] _aesKey;
        private readonly byte[] _aesIV;

        public ValidateTicketCommandHandlerTests()
        {
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Security:AES_Key", "AllEvents2024891" },
                { "Security:AES_IV", "E1F5D1A2C9B81234" }
            }).Build();

            _aesKey = Encoding.UTF8.GetBytes(_configuration["Security:AES_Key"]);
            _aesIV = Encoding.UTF8.GetBytes(_configuration["Security:AES_IV"]);

            _mockTicketRepository = new Mock<ITicketRepository>();

            _handler = new ValidateTicketCommandHandler(_configuration, _mockTicketRepository.Object);
        }

        private byte[] EncryptData(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _aesKey;
                aes.IV = _aesIV;
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
            var validTicketId = Guid.NewGuid();
            var validPersonName = "Valid Person";
            var invalidPersonName = "Invalid Person";

            var encryptedData = EncryptData($"{validTicketId}:{invalidPersonName}");
            var token = Convert.ToBase64String(encryptedData);

            var ticket = new Ticket(validTicketId, validPersonName, "Event Title", new byte[0], Guid.NewGuid());
            _mockTicketRepository.Setup(repo => repo.GetByIdAsync(validTicketId)).ReturnsAsync(ticket);

            // Act
            var result = await _handler.Handle(new ValidateTicketCommand { Token = token }, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Validation Failed", result.Message);
        }

        [Fact]
        public async Task ValidateTicketCommand_Should_Fail_For_Invalid_GUID()
        {
            // Arrange
            var invalidTicketId = Guid.NewGuid();
            var validPersonName = "Valid Person";

            var encryptedData = EncryptData($"{invalidTicketId}:{validPersonName}");
            var token = Convert.ToBase64String(encryptedData);

            _mockTicketRepository.Setup(repo => repo.GetByIdAsync(invalidTicketId)).ReturnsAsync((Ticket)null);

            // Act
            var result = await _handler.Handle(new ValidateTicketCommand { Token = token }, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Validation Failed", result.Message);
        }

        [Fact]
        public async Task ValidateTicketCommand_Should_Return_False_If_Ticket_Is_Null()
        {
            // Arrange
            var validTicketId = Guid.NewGuid();
            var validPersonName = "Valid Person";

            var encryptedData = EncryptData($"{validTicketId}:{validPersonName}");
            var token = Convert.ToBase64String(encryptedData);

            _mockTicketRepository.Setup(repo => repo.GetByIdAsync(validTicketId)).ReturnsAsync((Ticket)null);

            // Act
            var result = await _handler.Handle(new ValidateTicketCommand { Token = token }, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Validation Failed", result.Message);
        }

        [Fact]
        public async Task ValidateTicketCommand_Should_Return_False_For_Invalid_Token()
        {
            // Arrange
            // Act
            var result = await _handler.Handle(new ValidateTicketCommand { Token = "invalid-token" }, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Invalid Token", result.Message);
        }

        [Fact]
        public async Task ValidateTicketCommand_Should_Return_True_For_Valid_Token()
        {
            // Arrange
            var validTicketId = Guid.NewGuid();
            var validPersonName = "Valid Person";

            var encryptedData = EncryptData($"{validTicketId}:{validPersonName}");
            var token = Convert.ToBase64String(encryptedData);

            var ticket = new Ticket(validTicketId, validPersonName, "Event Title", new byte[0], Guid.NewGuid());
            _mockTicketRepository.Setup(repo => repo.GetByIdAsync(validTicketId)).ReturnsAsync(ticket);

            // Act
            var result = await _handler.Handle(new ValidateTicketCommand { Token = token }, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Equal("Validation Success", result.Message);
        }

        public void Dispose()
        {
            _mockTicketRepository.Reset();
        }
    }
}
