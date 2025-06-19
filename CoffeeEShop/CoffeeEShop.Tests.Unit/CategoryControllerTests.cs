using CoffeeEShop.Controllers;
using CoffeeEShop.Core;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CoffeeEShop.Tests.Unit;

public class CategoriesControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _controller = new CategoriesController(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedDatabase()
    {
        var categories = new[]
        {
            new ProductCategory { Id = 1, Name = "Brazil", Description = "From Brazil" },
            new ProductCategory { Id = 2, Name = "Colombia", Description = "From Colombia" }
        };
        _context.Categories.AddRange(categories);

        var products = new[]
        {
            new Product { Id = 1, Name = "Test Coffee", CategoryId = 1 }
        };
        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Arrange
        SeedDatabase();

        // Act & Assert
        var result = await _controller.GetAllCategoriesAsync();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IEnumerable<ProductCategory>>(okResult.Value);
        Assert.Equal(2, items.Count());
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ValidId_ReturnsCategory()
    {
        // Arrange
        SeedDatabase();

        // Act & Assert
        var result = await _controller.GetCategoryByIdAsync(1);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var item = Assert.IsType<ProductCategory>(okResult.Value);
        Assert.Equal("Brazil", item.Name);
    }

    [Fact]
    public async Task CreateCategoryAsync_ValidData_CreatesAndReturnsCategory()
    {
        // Arrange
        SeedDatabase(); 
        var dto = new CreateCategoryDto { Name = "Ethiopia", Description = "New coffee" };

        // Act & Assert
        var result = await _controller.CreateCategoryAsync(dto);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.IsType<ProductCategory>(createdAtActionResult.Value);
        Assert.Equal(3, await _context.Categories.CountAsync()); //
    }

    [Fact]
    public async Task UpdateCategoryAsync_ValidData_UpdatesAndReturnsCategory()
    {
        // Arrange
        SeedDatabase();
        var dto = new CreateCategoryDto { Name = "Updated Name", Description = "Updated Desc" };

        // Act & Assert
        var result = await _controller.UpdateCategoryAsync(1, dto);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var item = Assert.IsType<ProductCategory>(okResult.Value);
        Assert.Equal("Updated Name", item.Name);
    }

    [Fact]
    public async Task DeleteCategoryAsync_CategoryWithProducts_ReturnsBadRequest()
    {
        // Arrange
        SeedDatabase();

        // Act & Assert
        var result = await _controller.DeleteCategoryAsync(1);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ValidIdWithNoProducts_DeletesCategory()
    {
        // Arrange
        SeedDatabase();

        // Act & Assert
        var result = await _controller.DeleteCategoryAsync(2); 
        Assert.IsType<NoContentResult>(result);
    }
}