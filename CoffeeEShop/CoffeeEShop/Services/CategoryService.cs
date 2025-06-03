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

        public List<ProductCategory> GetAllCategories()
        {
            return _dataStore.Categories.ToList();
        }

        public ProductCategory? GetCategoryById(int id)
        {
            return _dataStore.Categories.FirstOrDefault(c => c.Id == id);
        }

        public ProductCategory CreateCategory(CreateCategoryDTO dto)
        {
            var category = new ProductCategory
            {
                Id = _dataStore.GetNextCategoryId(),
                Name = dto.Name,
                Description = dto.Description
            };

            _dataStore.Categories.Add(category);
            return category;
        }

        public ProductCategory? UpdateCategory(int id, CreateCategoryDTO dto)
        {
            var category = _dataStore.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return null;

            category.Name = dto.Name;
            category.Description = dto.Description;
            return category;
        }

        public bool DeleteCategory(int id)
        {
            var category = _dataStore.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return false;

            var hasProducts = _dataStore.Products.Any(p => p.CategoryId == id);
            if (hasProducts) return false;

            _dataStore.Categories.Remove(category);
            return true;
        }
    }
}

