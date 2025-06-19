using CoffeeEShop.Controllers;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeEShop.Tests.Unit;

public class ProductsControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _controller = new ProductsController(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedDatabase()
    {
        _context.Categories.Add(new ProductCategory { Id = 1, Name = "Brazil" });
        _context.Products.Add(new Product { Id = 1, Name = "Test Coffee", CategoryId = 1, Price = 10 });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllProductsAsync_WhenProductsExist_ReturnsAllProducts()
    {
        // Arrange
        SeedDatabase();

        // Act
        var result = await _controller.GetAllProductsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Product>>(okResult.Value);
        Assert.Single(returnedProducts);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_CreatesProduct()
    {
        // Arrange
        // No need to seed the database for this test
        _context.Categories.Add(new ProductCategory { Id = 1, Name = "Brazil" });
        _context.SaveChanges();

        var dto = new CreateProductDto
        {
            Name = "New Coffee",
            Price = 20,
            CategoryId = 1
        };

        // Act
        await _controller.CreateProductAsync(dto);

        // Assert
        Assert.Equal(1, _context.Products.Count());
        Assert.Equal("New Coffee", (await _context.Products.FirstAsync()).Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenIdIsInvalid_ReturnsNotFound()
    {
        // Arrange
        SeedDatabase();

        // Act
        var result = await _controller.GetProductByIdAsync(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenIdIsInvalid_ReturnsNotFound()
    {
        // Arrange
        SeedDatabase();
        var dto = new CreateProductDto { Name = "New Name", CategoryId = 1, Price = 15 };

        // Act
        var result = await _controller.UpdateProductAsync(999, dto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenIdIsInvalid_ReturnsNotFound()
    {
        // Arrange
        SeedDatabase();

        // Act
        var result = await _controller.DeleteProductAsync(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}