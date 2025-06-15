using CoffeeEShop.Application.Services.Interfaces;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CoffeeEShop.Application.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.Include(p => p.Category).ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateProductAsync(CreateProductDTO dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            IsAvailable = dto.IsAvailable,
            StockQuantity = dto.StockQuantity
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Return the product with the category loaded
        return (await GetProductByIdAsync(product.Id))!;
    }

    public async Task<Product?> UpdateProductAsync(int id, CreateProductDTO dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.CategoryId = dto.CategoryId;
        product.IsAvailable = dto.IsAvailable;
        product.StockQuantity = dto.StockQuantity;

        await _context.SaveChangesAsync();

        // Return the updated product with the category loaded
        return await GetProductByIdAsync(id);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        // Also remove the product from any baskets it's in
        var basketItemsToRemove = _context.BasketItems.Where(bi => bi.ProductId == id);
        _context.BasketItems.RemoveRange(basketItemsToRemove);

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();
        return true;
    }
}