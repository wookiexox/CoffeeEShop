using CoffeeEShop.Core;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeEShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAllProductsAsync()
    {
        var products = await _context.Products.Include(p => p.Category).ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id}", Name = "GetProductById")]
    public async Task<ActionResult<Product>> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Product>> CreateProductAsync([FromBody] CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Product name is required.");
        if (dto.Price <= 0) return BadRequest("Product price must be greater than 0.");
        var category = await _context.Categories.FindAsync(dto.CategoryId);
        if (category == null) return BadRequest("Invalid category ID.");

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
        return CreatedAtRoute(routeName: "GetProductById", routeValues: new { id = product.Id }, value: product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Product>> UpdateProductAsync(int id, [FromBody] CreateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound($"Product with ID {id} not found.");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.CategoryId = dto.CategoryId;
        product.IsAvailable = dto.IsAvailable;
        product.StockQuantity = dto.StockQuantity;

        await _context.SaveChangesAsync();
        return Ok(product);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound($"Product with ID {id} not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}