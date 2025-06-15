using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;

namespace CoffeeEShop.Application.Services.Interfaces
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