using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeEShop.Application.Services.DataStore;

namespace CoffeeEShop.Tests.Unit
{
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
