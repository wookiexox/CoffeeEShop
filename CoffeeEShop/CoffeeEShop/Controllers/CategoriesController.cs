using CoffeeEShop.Core;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeEShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllCategoriesAsync()
    {
        return Ok(await _context.Categories.ToListAsync());
    }

    [HttpGet("{id}", Name = "GetCategoryById")]
    public async Task<ActionResult<CreateCategoryDTO>> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound($"Category with ID {id} not found.");
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategoryAsync([FromBody] CreateCategoryDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Category name is required.");

        var category = new ProductCategory { Name = dto.Name, Description = dto.Description };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCategoryById", new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategoryAsync(int id, [FromBody] CreateCategoryDTO dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound($"Category with ID {id} not found.");

        category.Name = dto.Name;
        category.Description = dto.Description;
        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound($"Category with ID {id} not found.");

        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
        if (hasProducts) return BadRequest("Cannot delete category. It has associated products.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}