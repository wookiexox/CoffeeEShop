using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeEShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductsController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllProductsAsync()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}", Name = "GetProductById")]
    public async Task<ActionResult<CreateProductDTO>> GetProductByIdAsync(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult> CreateProductAsync([FromBody] CreateProductDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Product name is required.");

        if (dto.Price <= 0)
            return BadRequest("Product price must be greater than 0.");

        var category = await _categoryService.GetCategoryByIdAsync(dto.CategoryId);
        if (category == null)
            return BadRequest("Invalid category ID.");

        var product = await _productService.CreateProductAsync(dto);
        return CreatedAtRoute(routeName: "GetProductById", routeValues: new { id = product.Id }, value: product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProductAsync(int id, [FromBody] CreateProductDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Product name is required.");

        if (dto.Price <= 0)
            return BadRequest("Product price must be greater than 0.");

        var category = await _categoryService.GetCategoryByIdAsync(dto.CategoryId);
        if (category == null)
            return BadRequest("Invalid category ID.");

        var product = await _productService.UpdateProductAsync(id, dto);
        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProductAsync(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result)
            return NotFound($"Product with ID {id} not found.");

        return NoContent();
    }
}