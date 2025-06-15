using CoffeeEShop.Controllers;
using CoffeeEShop.Core;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CoffeeEShop.Tests.Unit;

public class ClientControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly ClientsController _controller;

    public ClientControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _controller = new ClientsController(_context);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var clients = new[]
        {
            new Client { Id = 1, FirstName = "John", LastName = "Doe" },
            new Client { Id = 2, FirstName = "Jane", LastName = "Smith" }
        };
        _context.Clients.AddRange(clients);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetClientByIdAsync_ValidId_ReturnsClient()
    {
        // Act
        var result = await _controller.GetClientByIdAsync(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var client = Assert.IsType<Client>(okResult.Value);
        Assert.Equal("John", client.FirstName);
    }

    [Fact]
    public async Task GetClientByIdAsync_InvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetClientByIdAsync(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}