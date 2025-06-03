using CoffeeEShop.Models;
using CoffeeEShop.Models.DTOs;

namespace CoffeeEShop.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<ProductCategory>> GetAllCategoriesAsync();
        Task<ProductCategory?> GetCategoryByIdAsync(int id);
        Task<ProductCategory> CreateCategoryAsync(CreateCategoryDTO dto);
        Task<ProductCategory?> UpdateCategoryAsync(int id, CreateCategoryDTO dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}