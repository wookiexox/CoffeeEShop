using CoffeeEShop.Models;

namespace CoffeeEShop.Services.DataStore
{
    public class InMemoryDataStore : IDataStore
    {
        private int _nextClientId = 1;
        private int _nextCategoryId = 1;
        private int _nextProductId = 1;
        private int _nextBasketItemId = 1;

        public List<Client> Clients { get; private set; }
        public List<ProductCategory> Categories { get; private set; }
        public List<Product> Products { get; private set; }
        public List<BasketItem> BasketItems { get; private set; }

        public InMemoryDataStore()
        {
            Clients = new List<Client>();
            Categories = new List<ProductCategory>();
            Products = new List<Product>();
            BasketItems = new List<BasketItem>();

            SeedData();
        }

        public int GetNextClientId() => _nextClientId++;
        public int GetNextCategoryId() => _nextCategoryId++;
        public int GetNextProductId() => _nextProductId++;
        public int GetNextBasketItemId() => _nextBasketItemId++;

        private void SeedData()
        {
            var categories = new[]
            {
                new ProductCategory { Id = GetNextCategoryId(), Name = "Brazil", Description = "Chocolate and hazelnut notes" },
                new ProductCategory { Id = GetNextCategoryId(), Name = "Columbia", Description = "Chocolate and carmel notes" },
                new ProductCategory { Id = GetNextCategoryId(), Name = "Ethiopia", Description = "Tea and herbal notes" },
                new ProductCategory { Id = GetNextCategoryId(), Name = "Kenya", Description = "Red fruits notes" }
            };
            Categories.AddRange(categories);

            var products = new[]
            {
                new Product { Id = GetNextProductId(), Name = "CoffeeLab", Description = "250g", Price = 40.00m, CategoryId = 1, IsAvailable = true, StockQuantity = 100 },
                new Product { Id = GetNextProductId(), Name = "Braziliana", Description = "500g", Price = 70.99m, CategoryId = 1, IsAvailable = true, StockQuantity = 80 },
                new Product { Id = GetNextProductId(), Name = "Qubana", Description = "1000g", Price = 120.75m, CategoryId = 1, IsAvailable = true, StockQuantity = 90 },
                new Product { Id = GetNextProductId(), Name = "ColCoffee", Description = "500g", Price = 52.25m, CategoryId = 2, IsAvailable = true, StockQuantity = 50 },
                new Product { Id = GetNextProductId(), Name = "EthCoffee", Description = "250g", Price = 21.37m, CategoryId = 3, IsAvailable = true, StockQuantity = 30 },
                new Product { Id = GetNextProductId(), Name = "CoffeeKenya", Description = "500g", Price = 111.33m, CategoryId = 4, IsAvailable = true, StockQuantity = 0 }
            };
            Products.AddRange(products);

            var clients = new[]
            {
                new Client { Id = GetNextClientId(), FirstName = "John", LastName = "Doe", Email = "john.doe@email.com", PhoneNumber = "123-456-7890" },
                new Client { Id = GetNextClientId(), FirstName = "Jane", LastName = "Smith", Email = "jane.smith@email.com", PhoneNumber = "098-765-4321" },
                new Client { Id = GetNextClientId(), FirstName = "Mike", LastName = "Johnson", Email = "mike.johnson@email.com", PhoneNumber = "555-123-4567" }
            };
            Clients.AddRange(clients);
        }
    }

}
