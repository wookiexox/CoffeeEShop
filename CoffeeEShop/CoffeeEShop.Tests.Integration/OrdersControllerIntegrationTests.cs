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

public class OrdersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OrdersControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Checkout_WithItemsInBasket_CreatesOrderAndReturnsOk()
    {
        // Arrange
        var authenticatedClient = _factory.CreateAuthenticatedClient();

        var basketDto = new CreateBasketItemDto { ClientId = 1, ProductId = 1, Quantity = 1 };
        await authenticatedClient.PostAsJsonAsync("/api/basket", basketDto);

        // Act
        var response = await authenticatedClient.PostAsync("/api/orders/checkout", null);

        // Assert
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true
        };

        var order = JsonSerializer.Deserialize<Order>(jsonString, jsonOptions);

        Assert.NotNull(order);
        Assert.Equal(1, order.ClientId);
        Assert.Equal(40.00m, order.TotalPrice);
        Assert.Single(order.OrderItems);
    }

    [Fact]
    public async Task Checkout_WhenStockIsInsufficient_ReturnsBadRequest()
    {
        // Arrange
        var authenticatedClient = _factory.CreateAuthenticatedClient();

        var basketDto = new CreateBasketItemDto { ClientId = 1, ProductId = 1, Quantity = 200 };
        await authenticatedClient.PostAsJsonAsync("/api/basket", basketDto);

        // Act
        var response = await authenticatedClient.PostAsync("/api/orders/checkout", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}