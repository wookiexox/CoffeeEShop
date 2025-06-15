using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeEShop.Core.Models.DTOs;
using Moq;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Application.Services;
using CoffeeEShop.Application.Services.DataStore;

namespace CoffeeEShop.Tests.Unit
{
    // ===== PRODUCT SERVICE TESTS =====
    public class ProductServiceTests
    {
        private readonly Mock<IDataStore> _mockDataStore;
        private readonly ProductService _productService;
        private readonly List<Product> _products;
        private readonly List<ProductCategory> _categories;
        private readonly List<BasketItem> _basketItems;

        public ProductServiceTests()
        {
            _mockDataStore = new Mock<IDataStore>();
            _productService = new ProductService(_mockDataStore.Object);

            _categories = new List<ProductCategory>
            {
                new ProductCategory { Id = 1, Name = "Brazil", Description = "Brazilian coffee" }
            };

            _products = new List<Product>
            {
                new Product { Id = 1, Name = "CoffeeLab", Description = "250g", Price = 40.00m, CategoryId = 1, IsAvailable = true, StockQuantity = 100 },
                new Product { Id = 2, Name = "Braziliana", Description = "500g", Price = 70.99m, CategoryId = 1, IsAvailable = false, StockQuantity = 0 }
            };

            _basketItems = new List<BasketItem>
            {
                new BasketItem { Id = 1, ClientId = 1, ProductId = 1, Quantity = 2 }
            };

            SetupMockDataStore();
        }

        private void SetupMockDataStore()
        {
            _mockDataStore.Setup(x => x.Products).Returns(_products);
            _mockDataStore.Setup(x => x.Categories).Returns(_categories);
            _mockDataStore.Setup(x => x.BasketItems).Returns(_basketItems);
            _mockDataStore.Setup(x => x.GetNextProductId()).Returns(() => _products.Count + 1);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProductsWithCategories()
        {
            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.NotNull(p.Category));
            Assert.Contains(result, p => p.Name == "CoffeeLab");
            Assert.Contains(result, p => p.Name == "Braziliana");
        }

        [Fact]
        public async Task GetProductByIdAsync_ValidId_ReturnsProductWithCategory()
        {
            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("CoffeeLab", result.Name);
            Assert.Equal(40.00m, result.Price);
            Assert.NotNull(result.Category);
            Assert.Equal("Brazil", result.Category.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _productService.GetProductByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProductAsync_ValidData_CreatesProductWithCategory()
        {
            // Arrange
            var dto = new CreateProductDTO
            {
                Name = "New Coffee",
                Description = "1kg",
                Price = 80.00m,
                CategoryId = 1,
                IsAvailable = true,
                StockQuantity = 50
            };

            // Act
            var result = await _productService.CreateProductAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Coffee", result.Name);
            Assert.Equal(80.00m, result.Price);
            Assert.Equal(3, result.Id);
            Assert.NotNull(result.Category);
            Assert.Equal(3, _products.Count);
        }

        [Fact]
        public async Task UpdateProductAsync_ValidData_UpdatesProductWithCategory()
        {
            // Arrange
            var dto = new CreateProductDTO
            {
                Name = "Updated CoffeeLab",
                Description = "Updated 250g",
                Price = 45.00m,
                CategoryId = 1,
                IsAvailable = false,
                StockQuantity = 75
            };

            // Act
            var result = await _productService.UpdateProductAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated CoffeeLab", result.Name);
            Assert.Equal(45.00m, result.Price);
            Assert.False(result.IsAvailable);
            Assert.Equal(75, result.StockQuantity);
            Assert.NotNull(result.Category);
        }

        [Fact]
        public async Task UpdateProductAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var dto = new CreateProductDTO
            {
                Name = "Updated",
                Description = "Updated",
                Price = 45.00m,
                CategoryId = 1,
                IsAvailable = true,
                StockQuantity = 75
            };

            // Act
            var result = await _productService.UpdateProductAsync(999, dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ValidId_DeletesProductAndRelatedBasketItems()
        {
            // Act
            var result = await _productService.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
            Assert.Single(_products);
            Assert.DoesNotContain(_products, p => p.Id == 1);
            Assert.Empty(_basketItems); // Related basket items should be removed
        }

        [Fact]
        public async Task DeleteProductAsync_InvalidId_ReturnsFalse()
        {
            // Act
            var result = await _productService.DeleteProductAsync(999);

            // Assert
            Assert.False(result);
            Assert.Equal(2, _products.Count);
            Assert.Single(_basketItems); // Basket items should remain unchanged
        }
    }
}
