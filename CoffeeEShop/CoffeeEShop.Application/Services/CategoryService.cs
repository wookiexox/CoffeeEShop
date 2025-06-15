using CoffeeEShop.Application.Services.Interfaces;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CoffeeEShop.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductCategory>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<ProductCategory?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<ProductCategory> CreateCategoryAsync(CreateCategoryDTO dto)
    {
        var category = new ProductCategory
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<ProductCategory?> UpdateCategoryAsync(int id, CreateCategoryDTO dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return null;

        category.Name = dto.Name;
        category.Description = dto.Description;

        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        // Prevent deletion if the category has associated products
        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
        if (hasProducts) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}