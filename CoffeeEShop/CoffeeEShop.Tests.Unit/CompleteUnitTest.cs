using CoffeeEShop.Models;
using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Services;
using CoffeeEShop.Services.DataStore;
using Moq;
using Xunit;

namespace CoffeeEShop.Tests.Unit.Services
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

    // ===== DATA STORE TESTS =====
    public class InMemoryDataStoreTests
    {
        private readonly InMemoryDataStore _dataStore;

        public InMemoryDataStoreTests()
        {
            _dataStore = new InMemoryDataStore();
        }

        [Fact]
        public void Constructor_InitializesWithSeedData()
        {
            // Assert
            Assert.NotEmpty(_dataStore.Categories);
            Assert.NotEmpty(_dataStore.Products);
            Assert.NotEmpty(_dataStore.Clients);
            Assert.Empty(_dataStore.BasketItems); // Should be empty initially
        }

        [Fact]
        public void Categories_ContainsSeedData()
        {
            // Assert
            Assert.Equal(4, _dataStore.Categories.Count);
            Assert.Contains(_dataStore.Categories, c => c.Name == "Brazil");
            Assert.Contains(_dataStore.Categories, c => c.Name == "Columbia");
            Assert.Contains(_dataStore.Categories, c => c.Name == "Ethiopia");
            Assert.Contains(_dataStore.Categories, c => c.Name == "Kenya");
        }

        [Fact]
        public void Products_ContainsSeedData()
        {
            // Assert
            Assert.Equal(6, _dataStore.Products.Count);
            Assert.Contains(_dataStore.Products, p => p.Name == "CoffeeLab");
            Assert.Contains(_dataStore.Products, p => p.Name == "Braziliana");
            Assert.Contains(_dataStore.Products, p => p.Name == "Qubana");
            Assert.Contains(_dataStore.Products, p => p.Name == "ColCoffee");
            Assert.Contains(_dataStore.Products, p => p.Name == "EthCoffee");
            Assert.Contains(_dataStore.Products, p => p.Name == "CoffeeKenya");
        }

        [Fact]
        public void Clients_ContainsSeedData()
        {
            // Assert
            Assert.Equal(3, _dataStore.Clients.Count);
            Assert.Contains(_dataStore.Clients, c => c.FirstName == "John" && c.LastName == "Doe");
            Assert.Contains(_dataStore.Clients, c => c.FirstName == "Jane" && c.LastName == "Smith");
            Assert.Contains(_dataStore.Clients, c => c.FirstName == "Mike" && c.LastName == "Johnson");
        }

        [Fact]
        public void GetNextClientId_ReturnsIncrementingIds()
        {
            // Act
            var id1 = _dataStore.GetNextClientId();
            var id2 = _dataStore.GetNextClientId();
            var id3 = _dataStore.GetNextClientId();

            // Assert
            Assert.Equal(4, id1); // Should start after seed data
            Assert.Equal(5, id2);
            Assert.Equal(6, id3);
        }

        [Fact]
        public void GetNextCategoryId_ReturnsIncrementingIds()
        {
            // Act
            var id1 = _dataStore.GetNextCategoryId();
            var id2 = _dataStore.GetNextCategoryId();

            // Assert
            Assert.Equal(5, id1); // Should start after seed data
            Assert.Equal(6, id2);
        }

        [Fact]
        public void GetNextProductId_ReturnsIncrementingIds()
        {
            // Act
            var id1 = _dataStore.GetNextProductId();
            var id2 = _dataStore.GetNextProductId();

            // Assert
            Assert.Equal(7, id1); // Should start after seed data
            Assert.Equal(8, id2);
        }

        [Fact]
        public void GetNextBasketItemId_ReturnsIncrementingIds()
        {
            // Act
            var id1 = _dataStore.GetNextBasketItemId();
            var id2 = _dataStore.GetNextBasketItemId();

            // Assert
            Assert.Equal(1, id1); // Should start from 1 (no seed data)
            Assert.Equal(2, id2);
        }

        [Fact]
        public void Products_HaveCorrectCategoryAssignments()
        {
            // Assert
            var brazilProducts = _dataStore.Products.Where(p => p.CategoryId == 1).ToList();
            var columbiaProducts = _dataStore.Products.Where(p => p.CategoryId == 2).ToList();
            var ethiopiaProducts = _dataStore.Products.Where(p => p.CategoryId == 3).ToList();
            var kenyaProducts = _dataStore.Products.Where(p => p.CategoryId == 4).ToList();

            Assert.Equal(3, brazilProducts.Count); // CoffeeLab, Braziliana, Qubana
            Assert.Single(columbiaProducts); // ColCoffee
            Assert.Single(ethiopiaProducts); // EthCoffee
            Assert.Single(kenyaProducts); // CoffeeKenya
        }

        [Fact]
        public void Products_HaveCorrectStockQuantities()
        {
            // Assert
            var outOfStockProduct = _dataStore.Products.FirstOrDefault(p => p.Name == "CoffeeKenya");
            var inStockProduct = _dataStore.Products.FirstOrDefault(p => p.Name == "CoffeeLab");

            Assert.NotNull(outOfStockProduct);
            Assert.NotNull(inStockProduct);
            Assert.Equal(0, outOfStockProduct.StockQuantity);
            Assert.Equal(100, inStockProduct.StockQuantity);
        }
    }
}