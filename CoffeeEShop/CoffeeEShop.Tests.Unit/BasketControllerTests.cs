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

public class BasketControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BasketController _controller;

    public BasketControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _controller = new BasketController(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddToBasketAsync_WithValidData_CreatesBasketItem()
    {
        // Arrange
        _context.Clients.Add(new Client { Id = 1, FirstName = "Test" });
        _context.Products.Add(new Product { Id = 1, Name = "Test Coffee", StockQuantity = 10 });
        _context.SaveChanges();

        var dto = new CreateBasketItemDto { ClientId = 1, ProductId = 1, Quantity = 2 };

        // Act
        var result = await _controller.AddToBasketAsync(dto);

        // Assert
        var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var item = Assert.IsType<BasketItem>(createdAtRouteResult.Value);
        Assert.Equal(1, _context.BasketItems.Count());
        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public async Task AddToBasketAsync_InsufficientStock_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateBasketItemDto { ClientId = 1, ProductId = 1, Quantity = 20 };

        // Act
        var result = await _controller.AddToBasketAsync(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task AddToBasketAsync_UnavailableProduct_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateBasketItemDto { ClientId = 1, ProductId = 2, Quantity = 1 };

        // Act
        var result = await _controller.AddToBasketAsync(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}