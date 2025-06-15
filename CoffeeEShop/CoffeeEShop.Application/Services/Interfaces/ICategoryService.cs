using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;

namespace CoffeeEShop.Application.Services.Interfaces
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