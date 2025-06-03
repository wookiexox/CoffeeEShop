using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Models;
using CoffeeEShop.Services.DataStore;
using CoffeeEShop.Services.Interfaces;

namespace CoffeeEShop.Services
{
    public class BasketService : IBasketService
    {
        private readonly IDataStore _dataStore;

        public BasketService(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<List<BasketItem>> GetBasketByClientIdAsync(int clientId)
        {
            await Task.Delay(1); // Simulate async operation

            var basketItems = _dataStore.BasketItems.Where(bi => bi.ClientId == clientId).ToList();
            foreach (var item in basketItems)
            {
                item.Product = _dataStore.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (item.Product != null)
                {
                    item.Product.Category = _dataStore.Categories.FirstOrDefault(c => c.Id == item.Product.CategoryId);
                }
            }
            return basketItems;
        }

        public async Task<BasketItem?> GetBasketItemByIdAsync(int id)
        {
            await Task.Delay(1); // Simulate async operation

            var basketItem = _dataStore.BasketItems.FirstOrDefault(bi => bi.Id == id);
            if (basketItem != null)
            {
                basketItem.Product = _dataStore.Products.FirstOrDefault(p => p.Id == basketItem.ProductId);
                if (basketItem.Product != null)
                {
                    basketItem.Product.Category = _dataStore.Categories.FirstOrDefault(c => c.Id == basketItem.Product.CategoryId);
                }
            }
            return basketItem;
        }

        public async Task<BasketItem?> AddToBasketAsync(CreateBasketItemDTO dto)
        {
            await Task.Delay(1); // Simulate async operation

            // Check if product exists and is available
            var product = _dataStore.Products.FirstOrDefault(p => p.Id == dto.ProductId);
            if (product == null || !product.IsAvailable || product.StockQuantity < dto.Quantity)
                return null;

            // Check if client exists
            var client = _dataStore.Clients.FirstOrDefault(c => c.Id == dto.ClientId);
            if (client == null) return null;

            // Check if item already exists in basket
            var existingItem = _dataStore.BasketItems.FirstOrDefault(bi => bi.ClientId == dto.ClientId && bi.ProductId == dto.ProductId);
            if (existingItem != null)
            {
                // Check if total quantity would exceed stock
                if (product.StockQuantity < existingItem.Quantity + dto.Quantity)
                    return null;

                existingItem.Quantity += dto.Quantity;
                existingItem.Product = product;
                product.Category = _dataStore.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
                return existingItem;
            }

            var basketItem = new BasketItem
            {
                Id = _dataStore.GetNextBasketItemId(),
                ClientId = dto.ClientId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                AddedAt = DateTime.Now
            };

            _dataStore.BasketItems.Add(basketItem);
            basketItem.Product = product;
            product.Category = _dataStore.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            return basketItem;
        }

        public async Task<BasketItem?> UpdateBasketItemAsync(int id, int quantity)
        {
            await Task.Delay(1); // Simulate async operation

            var basketItem = _dataStore.BasketItems.FirstOrDefault(bi => bi.Id == id);
            if (basketItem == null) return null;

            var product = _dataStore.Products.FirstOrDefault(p => p.Id == basketItem.ProductId);
            if (product == null || product.StockQuantity < quantity) return null;

            basketItem.Quantity = quantity;
            basketItem.Product = product;
            product.Category = _dataStore.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            return basketItem;
        }

        public async Task<bool> RemoveFromBasketAsync(int id)
        {
            await Task.Delay(1); // Simulate async operation

            var basketItem = _dataStore.BasketItems.FirstOrDefault(bi => bi.Id == id);
            if (basketItem == null) return false;

            _dataStore.BasketItems.Remove(basketItem);
            return true;
        }

        public async Task<bool> ClearBasketAsync(int clientId)
        {
            await Task.Delay(1); // Simulate async operation

            var basketItems = _dataStore.BasketItems.Where(bi => bi.ClientId == clientId).ToList();
            foreach (var item in basketItems)
            {
                _dataStore.BasketItems.Remove(item);
            }
            return true;
        }
    }
}