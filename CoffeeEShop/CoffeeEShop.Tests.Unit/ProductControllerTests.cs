using CoffeeEShop.Controllers;
using CoffeeEShop.Core;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CoffeeEShop.Tests.Unit;

public class ProductsControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        // 1. Set up the in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test run
            .Options;
        _context = new ApplicationDbContext(options);

        // 2. Instantiate the controller with the test database context
        _controller = new ProductsController(_context);

        // 3. Seed the database with initial data for tests
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var categories = new[]
        {
            new ProductCategory { Id = 1, Name = "Brazil", Description = "Brazilian coffee" }
        };
        _context.Categories.AddRange(categories);

        var products = new[]
        {
            new Product { Id = 1, Name = "CoffeeLab", Description = "250g", Price = 40.00m, CategoryId = 1, IsAvailable = true, StockQuantity = 100 },
            new Product { Id = 2, Name = "Braziliana", Description = "500g", Price = 70.99m, CategoryId = 1, IsAvailable = false, StockQuantity = 0 }
        };
        _context.Products.AddRange(products);
        _context.SaveChanges();
    }


    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Act
        var result = await _controller.GetAllProductsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Product>>(okResult.Value);
        Assert.Equal(2, returnedProducts.Count());
    }

    [Fact]
    public async Task GetProductByIdAsync_ValidId_ReturnsProduct()
    {
        // Act
        var result = await _controller.GetProductByIdAsync(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var product = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(1, product.Id);
        Assert.Equal("CoffeeLab", product.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_InvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetProductByIdAsync(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }


    [Fact]
    public async Task CreateProductAsync_ValidData_CreatesProduct()
    {
        // Arrange
        var dto = new CreateProductDTO
        {
            Name = "New Coffee",
            Price = 80.00m,
            CategoryId = 1,
            StockQuantity = 50,
            IsAvailable = true
        };

        // Act
        var result = await _controller.CreateProductAsync(dto);

        // Assert
        var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var product = Assert.IsType<Product>(createdAtRouteResult.Value);
        Assert.Equal("New Coffee", product.Name);
        Assert.Equal(3, _context.Products.Count()); // Verify it was added to the DB
    }

    [Fact]
    public async Task UpdateProductAsync_ValidData_UpdatesProduct()
    {
        // Arrange
        var dto = new CreateProductDTO
        {
            Name = "Updated CoffeeLab",
            Price = 45.00m,
            CategoryId = 1
        };

        // Act
        var result = await _controller.UpdateProductAsync(1, dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var product = Assert.IsType<Product>(okResult.Value);
        Assert.Equal("Updated CoffeeLab", product.Name);
        Assert.Equal(45.00m, product.Price);
    }

    [Fact]
    public async Task DeleteProductAsync_ValidId_DeletesProduct()
    {
        // Act
        var result = await _controller.DeleteProductAsync(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(1, _context.Products.Count()); // Verify it was removed from the DB
        Assert.Null(await _context.Products.FindAsync(1));
    }
}