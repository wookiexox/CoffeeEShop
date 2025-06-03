using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Models;
using CoffeeEShop.Services.DataStore;
using CoffeeEShop.Services.Interfaces;

namespace CoffeeEShop.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IDataStore _dataStore;

        public CategoryService(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<List<ProductCategory>> GetAllCategoriesAsync()
        {
            await Task.Delay(1); // Simulate async operation
            return _dataStore.Categories.ToList();
        }

        public async Task<ProductCategory?> GetCategoryByIdAsync(int id)
        {
            await Task.Delay(1); // Simulate async operation
            return _dataStore.Categories.FirstOrDefault(c => c.Id == id);
        }

        public async Task<ProductCategory> CreateCategoryAsync(CreateCategoryDTO dto)
        {
            await Task.Delay(1); // Simulate async operation

            var category = new ProductCategory
            {
                Id = _dataStore.GetNextCategoryId(),
                Name = dto.Name,
                Description = dto.Description
            };

            _dataStore.Categories.Add(category);
            return category;
        }

        public async Task<ProductCategory?> UpdateCategoryAsync(int id, CreateCategoryDTO dto)
        {
            await Task.Delay(1); // Simulate async operation

            var category = _dataStore.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return null;

            category.Name = dto.Name;
            category.Description = dto.Description;
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            await Task.Delay(1); // Simulate async operation

            var category = _dataStore.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return false;

            var hasProducts = _dataStore.Products.Any(p => p.CategoryId == id);
            if (hasProducts) return false;

            _dataStore.Categories.Remove(category);
            return true;
        }
    }
}