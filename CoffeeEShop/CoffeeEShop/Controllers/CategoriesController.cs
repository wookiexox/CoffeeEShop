using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeEShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllCategoriesAsync()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}", Name = "GetCategoryById")]
    public async Task<ActionResult<CreateCategoryDTO>> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound($"Category with ID {id} not found.");

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategoryAsync([FromBody] CreateCategoryDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Category name is required.");

        var category = await _categoryService.CreateCategoryAsync(dto);
        return CreatedAtAction("GetCategoryById", new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategoryAsync(int id, [FromBody] CreateCategoryDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Category name is required.");

        var category = await _categoryService.UpdateCategoryAsync(id, dto);
        if (category == null)
            return NotFound($"Category with ID {id} not found.");

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategoryAsync(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result)
            return BadRequest("Cannot delete category. It may not exist or has associated products.");

        return NoContent();
    }
}