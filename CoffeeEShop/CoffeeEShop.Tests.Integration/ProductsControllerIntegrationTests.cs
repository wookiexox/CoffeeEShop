using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Tests.Integration.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoffeeEShop.Tests.Integration;

public class ProductsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ProductsControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }

    [Fact]
    public async Task GetAllProducts_ReturnsInitialProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products");
        var jsonString = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true
        };

        using var jsonDoc = JsonDocument.Parse(jsonString);
        var valuesElement = jsonDoc.RootElement.GetProperty("$values");
        var products = JsonSerializer.Deserialize<List<Product>>(valuesElement.GetRawText(), jsonOptions);

        Assert.NotNull(products);
        Assert.Equal(6, products.Count);
        Assert.Contains(products, p => p.Name == "CoffeeLab");
    }

    [Fact]
    public async Task CreateProduct_AsAnonymousUser_ReturnsUnauthorized()
    {
        // Arrange
        var newProduct = new CreateProductDto { Name = "Unauthorized Coffee", Price = 10, CategoryId = 1 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var userClient = _factory.CreateAuthenticatedClient(role: "User");
        var newProduct = new CreateProductDto { Name = "Forbidden Coffee", Price = 10, CategoryId = 1 };

        // Act
        var response = await userClient.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_AsAdmin_ReturnsCreated()
    {
        // Arrange
        var adminClient = _factory.CreateAuthenticatedClient(role: "Admin");
        var newProduct = new CreateProductDto { Name = "Admin's Coffee", Price = 100, CategoryId = 1 };

        // Act
        var response = await adminClient.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
