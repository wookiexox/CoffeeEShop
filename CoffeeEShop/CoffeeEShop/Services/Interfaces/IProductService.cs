using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Models;

namespace CoffeeEShop.Services.Interfaces
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product? GetProductById(int id);
        Product CreateProduct(CreateProductDTO dto);
        Product? UpdateProduct(int id, CreateProductDTO dto);
        bool DeleteProduct(int id);
    }
}
