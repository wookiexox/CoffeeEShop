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
    public ActionResult GetAllCategories()
    {
        var categories = _categoryService.GetAllCategories();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public ActionResult GetCategoryById(int id)
    {
        var category = _categoryService.GetCategoryById(id);
        if (category == null)
            return NotFound($"Category with ID {id} not found.");

        return Ok(category);
    }

    [HttpPost]
    public ActionResult CreateCategory([FromBody] CreateCategoryDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Category name is required.");

        var category = _categoryService.CreateCategory(dto);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateCategory(int id, [FromBody] CreateCategoryDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Category name is required.");

        var category = _categoryService.UpdateCategory(id, dto);
        if (category == null)
            return NotFound($"Category with ID {id} not found.");

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCategory(int id)
    {
        var result = _categoryService.DeleteCategory(id);
        if (!result)
            return BadRequest("Cannot delete category. It may not exist or has associated products.");

        return NoContent();
    }
}