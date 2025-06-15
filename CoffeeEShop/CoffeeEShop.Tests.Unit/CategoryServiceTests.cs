using CoffeeEShop.Application.Services;
using CoffeeEShop.Application.Services.DataStore;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using Moq;
using Xunit;

namespace CoffeeEShop.Tests.Unit
{
    // ===== CATEGORY SERVICE TESTS =====
    public class CategoryServiceTests
    {
        private readonly Mock<IDataStore> _mockDataStore;
        private readonly CategoryService _categoryService;
        private readonly List<ProductCategory> _categories;
        private readonly List<Product> _products;

        public CategoryServiceTests()
        {
            _mockDataStore = new Mock<IDataStore>();
            _categoryService = new CategoryService(_mockDataStore.Object);

            _categories = new List<ProductCategory>
            {
                new ProductCategory { Id = 1, Name = "Brazil", Description = "Brazilian coffee" },
                new ProductCategory { Id = 2, Name = "Colombia", Description = "Colombian coffee" }
            };

            _products = new List<Product>
            {
                new Product { Id = 1, Name = "Test Product", CategoryId = 1, Price = 10.00m }
            };

            SetupMockDataStore();
        }

        private void SetupMockDataStore()
        {
            _mockDataStore.Setup(x => x.Categories).Returns(_categories);
            _mockDataStore.Setup(x => x.Products).Returns(_products);
            _mockDataStore.Setup(x => x.GetNextCategoryId()).Returns(() => _categories.Count + 1);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Brazil");
            Assert.Contains(result, c => c.Name == "Colombia");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ValidId_ReturnsCategory()
        {
            // Act
            var result = await _categoryService.GetCategoryByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Brazil", result.Name);
            Assert.Equal("Brazilian coffee", result.Description);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _categoryService.GetCategoryByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCategoryAsync_ValidData_CreatesCategory()
        {
            // Arrange
            var dto = new CreateCategoryDTO { Name = "Ethiopia", Description = "Ethiopian coffee" };

            // Act
            var result = await _categoryService.CreateCategoryAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Ethiopia", result.Name);
            Assert.Equal("Ethiopian coffee", result.Description);
            Assert.Equal(3, result.Id);
            Assert.Equal(3, _categories.Count);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ValidData_UpdatesCategory()
        {
            // Arrange
            var dto = new CreateCategoryDTO { Name = "Updated Brazil", Description = "Updated description" };

            // Act
            var result = await _categoryService.UpdateCategoryAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Brazil", result.Name);
            Assert.Equal("Updated description", result.Description);
            Assert.Equal("Updated Brazil", _categories[0].Name);
        }

        [Fact]
        public async Task UpdateCategoryAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var dto = new CreateCategoryDTO { Name = "Updated", Description = "Updated description" };

            // Act
            var result = await _categoryService.UpdateCategoryAsync(999, dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ValidIdNoProducts_DeletesCategory()
        {
            // Act
            var result = await _categoryService.DeleteCategoryAsync(2); // Category without products

            // Assert
            Assert.True(result);
            Assert.Single(_categories);
            Assert.DoesNotContain(_categories, c => c.Id == 2);
        }

        [Fact]
        public async Task DeleteCategoryAsync_CategoryWithProducts_ReturnsFalse()
        {
            // Act
            var result = await _categoryService.DeleteCategoryAsync(1); // Category with products

            // Assert
            Assert.False(result);
            Assert.Equal(2, _categories.Count);
        }

        [Fact]
        public async Task DeleteCategoryAsync_InvalidId_ReturnsFalse()
        {
            // Act
            var result = await _categoryService.DeleteCategoryAsync(999);

            // Assert
            Assert.False(result);
            Assert.Equal(2, _categories.Count);
        }
    }
}