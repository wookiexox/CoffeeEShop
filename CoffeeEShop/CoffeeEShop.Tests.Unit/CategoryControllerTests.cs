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

public class CategoriesControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        // Setup a unique in-memory database for each test run to ensure isolation
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _controller = new CategoriesController(_context);

        SeedDatabase();
    }

    // Seeds the in-memory database with initial data
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
            // This product is linked to the "Brazil" category
            new Product { Id = 1, Name = "Test Coffee", CategoryId = 1 }
        };
        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Act
        var result = await _controller.GetAllCategoriesAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IEnumerable<ProductCategory>>(okResult.Value);
        Assert.Equal(2, items.Count());
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ValidId_ReturnsCategory()
    {
        // Act
        var result = await _controller.GetCategoryByIdAsync(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var item = Assert.IsType<ProductCategory>(okResult.Value);
        Assert.Equal("Brazil", item.Name);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_InvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetCategoryByIdAsync(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateCategoryAsync_ValidData_CreatesAndReturnsCategory()
    {
        // Arrange
        var dto = new CreateCategoryDTO { Name = "Ethiopia", Description = "New coffee" };

        // Act
        var result = await _controller.CreateCategoryAsync(dto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var item = Assert.IsType<ProductCategory>(createdAtActionResult.Value);
        Assert.Equal("Ethiopia", item.Name);
        Assert.Equal(3, await _context.Categories.CountAsync());
    }

    [Fact]
    public async Task UpdateCategoryAsync_ValidData_UpdatesAndReturnsCategory()
    {
        // Arrange
        var dto = new CreateCategoryDTO { Name = "Updated Name", Description = "Updated Desc" };

        // Act
        var result = await _controller.UpdateCategoryAsync(1, dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var item = Assert.IsType<ProductCategory>(okResult.Value);
        Assert.Equal("Updated Name", item.Name);
    }

    [Fact]
    public async Task UpdateCategoryAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var dto = new CreateCategoryDTO { Name = "Does not matter" };

        // Act
        var result = await _controller.UpdateCategoryAsync(999, dto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_CategoryWithProducts_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.DeleteCategoryAsync(1); // This category has a product

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(2, await _context.Categories.CountAsync()); // Ensure nothing was deleted
    }

    [Fact]
    public async Task DeleteCategoryAsync_ValidIdWithNoProducts_DeletesCategory()
    {
        // Act
        var result = await _controller.DeleteCategoryAsync(2); // This category has no products

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(1, await _context.Categories.CountAsync()); // Ensure one was deleted
    }

    [Fact]
    public async Task DeleteCategoryAsync_InvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.DeleteCategoryAsync(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}