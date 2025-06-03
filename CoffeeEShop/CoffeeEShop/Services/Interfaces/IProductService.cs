using CoffeeEShop.Models;
using CoffeeEShop.Models.DTOs;

namespace CoffeeEShop.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(CreateProductDTO dto);
        Task<Product?> UpdateProductAsync(int id, CreateProductDTO dto);
        Task<bool> DeleteProductAsync(int id);
    }
}