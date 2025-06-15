using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Application.Services.DataStore;
using CoffeeEShop.Application.Services;

namespace CoffeeEShop.Tests.Unit
{
    // ===== CLIENT SERVICE TESTS =====
    public class ClientServiceTests
    {
        private readonly Mock<IDataStore> _mockDataStore;
        private readonly ClientService _clientService;
        private readonly List<Client> _clients;

        public ClientServiceTests()
        {
            _mockDataStore = new Mock<IDataStore>();
            _clientService = new ClientService(_mockDataStore.Object);

            _clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@email.com", PhoneNumber = "123-456-7890" },
                new Client { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@email.com", PhoneNumber = "098-765-4321" },
                new Client { Id = 3, FirstName = "Mike", LastName = "Johnson", Email = "mike.johnson@email.com", PhoneNumber = "555-123-4567" }
            };

            SetupMockDataStore();
        }

        private void SetupMockDataStore()
        {
            _mockDataStore.Setup(x => x.Clients).Returns(_clients);
        }

        [Fact]
        public async Task GetAllClientsAsync_ReturnsAllClients()
        {
            // Act
            var result = await _clientService.GetAllClientsAsync();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, c => c.FirstName == "John" && c.LastName == "Doe");
            Assert.Contains(result, c => c.FirstName == "Jane" && c.LastName == "Smith");
            Assert.Contains(result, c => c.FirstName == "Mike" && c.LastName == "Johnson");
        }

        [Fact]
        public async Task GetClientByIdAsync_ValidId_ReturnsClient()
        {
            // Act
            var result = await _clientService.GetClientByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john.doe@email.com", result.Email);
            Assert.Equal("123-456-7890", result.PhoneNumber);
        }

        [Fact]
        public async Task GetClientByIdAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _clientService.GetClientByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetClientByIdAsync_MultipleClients_ReturnsCorrectClient()
        {
            // Act
            var result1 = await _clientService.GetClientByIdAsync(1);
            var result2 = await _clientService.GetClientByIdAsync(2);
            var result3 = await _clientService.GetClientByIdAsync(3);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);

            Assert.Equal("John", result1.FirstName);
            Assert.Equal("Jane", result2.FirstName);
            Assert.Equal("Mike", result3.FirstName);

            Assert.Equal("Doe", result1.LastName);
            Assert.Equal("Smith", result2.LastName);
            Assert.Equal("Johnson", result3.LastName);
        }
    }
}
