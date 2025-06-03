using CoffeeEShop.Models;
using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Services;
using CoffeeEShop.Services.DataStore;
using Moq;
using Xunit;

namespace CoffeeEShop.Tests.Unit.Services
{
    public class BasketServiceTests
    {
        private readonly Mock<IDataStore> _mockDataStore;
        private readonly BasketService _basketService;
        private readonly List<Client> _clients;
        private readonly List<Product> _products;
        private readonly List<ProductCategory> _categories;
        private readonly List<BasketItem> _basketItems;

        public BasketServiceTests()
        {
            _mockDataStore = new Mock<IDataStore>();
            _basketService = new BasketService(_mockDataStore.Object);

            // Setup test data
            _categories = new List<ProductCategory>
            {
                new ProductCategory { Id = 1, Name = "Brazil", Description = "Brazilian coffee" }
            };

            _products = new List<Product>
            {
                new Product { Id = 1, Name = "CoffeeLab", Price = 40.00m, CategoryId = 1, IsAvailable = true, StockQuantity = 100 },
                new Product { Id = 2, Name = "OutOfStock", Price = 50.00m, CategoryId = 1, IsAvailable = true, StockQuantity = 0 }
            };

            _clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" }
            };

            _basketItems = new List<BasketItem>();

            SetupMockDataStore();
        }

        private void SetupMockDataStore()
        {
            _mockDataStore.Setup(x => x.Clients).Returns(_clients);
            _mockDataStore.Setup(x => x.Products).Returns(_products);
            _mockDataStore.Setup(x => x.Categories).Returns(_categories);
            _mockDataStore.Setup(x => x.BasketItems).Returns(_basketItems);
            _mockDataStore.Setup(x => x.GetNextBasketItemId()).Returns(() => _basketItems.Count + 1);
        }

        [Fact]
        public async Task GetBasketByClientIdAsync_ValidClient_ReturnsBasketItems()
        {
            // Arrange
            var clientId = 1;
            _basketItems.Add(new BasketItem { Id = 1, ClientId = clientId, ProductId = 1, Quantity = 2 });

            // Act
            var result = await _basketService.GetBasketByClientIdAsync(clientId);

            // Assert
            Assert.Single(result);
            Assert.Equal(clientId, result[0].ClientId);
            Assert.NotNull(result[0].Product);
            Assert.NotNull(result[0].Product.Category);
        }

        [Fact]
        public async Task GetBasketItemByIdAsync_ValidId_ReturnsBasketItem()
        {
            // Arrange
            var basketItem = new BasketItem { Id = 1, ClientId = 1, ProductId = 1, Quantity = 2 };
            _basketItems.Add(basketItem);

            // Act
            var result = await _basketService.GetBasketItemByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.NotNull(result.Product);
            Assert.NotNull(result.Product.Category);
        }

        [Fact]
        public async Task GetBasketItemByIdAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _basketService.GetBasketItemByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddToBasketAsync_ValidData_AddsNewItem()
        {
            // Arrange
            var dto = new CreateBasketItemDTO { ClientId = 1, ProductId = 1, Quantity = 2 };

            // Act
            var result = await _basketService.AddToBasketAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.ClientId, result.ClientId);
            Assert.Equal(dto.ProductId, result.ProductId);
            Assert.Equal(dto.Quantity, result.Quantity);
            Assert.Single(_basketItems);
        }

        [Fact]
        public async Task AddToBasketAsync_ExistingItem_UpdatesQuantity()
        {
            // Arrange
            _basketItems.Add(new BasketItem { Id = 1, ClientId = 1, ProductId = 1, Quantity = 2 });
            var dto = new CreateBasketItemDTO { ClientId = 1, ProductId = 1, Quantity = 3 };

            // Act
            var result = await _basketService.AddToBasketAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Quantity); // 2 + 3
            Assert.Single(_basketItems); // No new item added
        }

        [Fact]
        public async Task AddToBasketAsync_InsufficientStock_ReturnsNull()
        {
            // Arrange
            var dto = new CreateBasketItemDTO { ClientId = 1, ProductId = 1, Quantity = 150 }; // More than stock (100)

            // Act
            var result = await _basketService.AddToBasketAsync(dto);

            // Assert
            Assert.Null(result);
            Assert.Empty(_basketItems);
        }

        [Fact]
        public async Task AddToBasketAsync_OutOfStockProduct_ReturnsNull()
        {
            // Arrange
            var dto = new CreateBasketItemDTO { ClientId = 1, ProductId = 2, Quantity = 1 }; // Product with 0 stock

            // Act
            var result = await _basketService.AddToBasketAsync(dto);

            // Assert
            Assert.Null(result);
            Assert.Empty(_basketItems);
        }

        [Fact]
        public async Task AddToBasketAsync_InvalidClient_ReturnsNull()
        {
            // Arrange
            var dto = new CreateBasketItemDTO { ClientId = 999, ProductId = 1, Quantity = 1 };

            // Act
            var result = await _basketService.AddToBasketAsync(dto);

            // Assert
            Assert.Null(result);
            Assert.Empty(_basketItems);
        }

        [Fact]
        public async Task AddToBasketAsync_InvalidProduct_ReturnsNull()
        {
            // Arrange
            var dto = new CreateBasketItemDTO { ClientId = 1, ProductId = 999, Quantity = 1 };

            // Act
            var result = await _basketService.AddToBasketAsync(dto);

            // Assert
            Assert.Null(result);
            Assert.Empty(_basketItems);
        }

        [Fact]
        public async Task UpdateBasketItemAsync_ValidData_UpdatesQuantity()
        {
            // Arrange
            var basketItem = new BasketItem { Id = 1, ClientId = 1, ProductId = 1, Quantity = 2 };
            _basketItems.Add(basketItem);

            // Act
            var result = await _basketService.UpdateBasketItemAsync(1, 5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Quantity);
        }

        [Fact]
        public async Task UpdateBasketItemAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _basketService.UpdateBasketItemAsync(999, 5);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateBasketItemAsync_InsufficientStock_ReturnsNull()
        {
            // Arrange
            var basketItem = new BasketItem { Id = 1, ClientId = 1, ProductId = 1, Quantity = 2 };
            _basketItems.Add(basketItem);

            // Act
            var result = await _basketService.UpdateBasketItemAsync(1, 150); // More than stock

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveFromBasketAsync_ValidId_RemovesItem()
        {
            // Arrange
            var basketItem = new BasketItem { Id = 1, ClientId = 1, ProductId = 1, Quantity = 2 };
            _basketItems.Add(basketItem);

            // Act
            var result = await _basketService.RemoveFromBasketAsync(1);

            // Assert
            Assert.True(result);
            Assert.Empty(_basketItems);
        }

        [Fact]
        public async Task RemoveFromBasketAsync_InvalidId_ReturnsFalse()
        {
            // Act
            var result = await _basketService.RemoveFromBasketAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ClearBasketAsync_ValidClient_RemovesAllItems()
        {
            // Arrange
            _basketItems.Add(new BasketItem { Id = 1, ClientId = 1, ProductId = 1, Quantity = 2 });
            _basketItems.Add(new BasketItem { Id = 2, ClientId = 1, ProductId = 2, Quantity = 3 });
            _basketItems.Add(new BasketItem { Id = 3, ClientId = 2, ProductId = 1, Quantity = 1 }); // Different client

            // Act
            var result = await _basketService.ClearBasketAsync(1);

            // Assert
            Assert.True(result);
            Assert.Single(_basketItems); // Only item from client 2 should remain
            Assert.Equal(2, _basketItems[0].ClientId);
        }
    }
}