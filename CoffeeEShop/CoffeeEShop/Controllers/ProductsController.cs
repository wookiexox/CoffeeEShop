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
    public ActionResult GetAllProducts()
    {
        var products = _productService.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public ActionResult GetProductById(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        return Ok(product);
    }

    [HttpPost]
    public ActionResult CreateProduct([FromBody] CreateProductDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Product name is required.");

        if (dto.Price <= 0)
            return BadRequest("Product price must be greater than 0.");

        var category = _categoryService.GetCategoryById(dto.CategoryId);
        if (category == null)
            return BadRequest("Invalid category ID.");

        var product = _productService.CreateProduct(dto);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateProduct(int id, [FromBody] CreateProductDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Product name is required.");

        if (dto.Price <= 0)
            return BadRequest("Product price must be greater than 0.");

        var category = _categoryService.GetCategoryById(dto.CategoryId);
        if (category == null)
            return BadRequest("Invalid category ID.");

        var product = _productService.UpdateProduct(id, dto);
        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteProduct(int id)
    {
        var result = _productService.DeleteProduct(id);
        if (!result)
            return NotFound($"Product with ID {id} not found.");

        return NoContent();
    }
}