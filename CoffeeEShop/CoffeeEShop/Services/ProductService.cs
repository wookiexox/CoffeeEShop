using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Models;
using CoffeeEShop.Services.DataStore;
using CoffeeEShop.Services.Interfaces;

namespace CoffeeEShop.Services
{
    public class ProductService : IProductService
    {
        private readonly IDataStore _dataStore;

        public ProductService(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            await Task.Delay(1); // Simulate async operation

            var products = _dataStore.Products.ToList();
            foreach (var product in products)
            {
                product.Category = _dataStore.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }
            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            await Task.Delay(1); // Simulate async operation

            var product = _dataStore.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.Category = _dataStore.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }
            return product;
        }

        public async Task<Product> CreateProductAsync(CreateProductDTO dto)
        {
            await Task.Delay(1); // Simulate async operation

            var product = new Product
            {
                Id = _dataStore.GetNextProductId(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                IsAvailable = dto.IsAvailable,
                StockQuantity = dto.StockQuantity
            };

            _dataStore.Products.Add(product);
            product.Category = _dataStore.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            return product;
        }

        public async Task<Product?> UpdateProductAsync(int id, CreateProductDTO dto)
        {
            await Task.Delay(1); // Simulate async operation

            var product = _dataStore.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return null;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.IsAvailable = dto.IsAvailable;
            product.StockQuantity = dto.StockQuantity;
            product.Category = _dataStore.Categories.FirstOrDefault(c => c.Id == product.CategoryId);

            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            await Task.Delay(1); // Simulate async operation

            var product = _dataStore.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return false;

            var basketItemsToRemove = _dataStore.BasketItems.Where(bi => bi.ProductId == id).ToList();
            foreach (var item in basketItemsToRemove)
            {
                _dataStore.BasketItems.Remove(item);
            }

            _dataStore.Products.Remove(product);
            return true;
        }
    }
}