using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Models;

namespace CoffeeEShop.Services.Interfaces
{
    public interface ICategoryService
    {
        List<ProductCategory> GetAllCategories();
        ProductCategory? GetCategoryById(int id);
        ProductCategory CreateCategory(CreateCategoryDTO dto);
        ProductCategory? UpdateCategory(int id, CreateCategoryDTO dto);
        bool DeleteCategory(int id);

    }
}
