using CoffeeEShop.Core.DTOs.Auth;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeEShop.Tests.Integration;

public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithNewEmail_ReturnsSuccess()
    {
        // Arrange
        var newUser = new UserRegisterDto
        {
            Email = $"testuser_{Guid.NewGuid()}@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", newUser);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdClient = await response.Content.ReadFromJsonAsync<Client>();
        Assert.NotNull(createdClient);
        Assert.Equal(newUser.Email, createdClient.Email);
        Assert.Equal(newUser.FirstName, createdClient.FirstName);
        Assert.Equal("User", createdClient.Role);
        Assert.NotEqual(0, createdClient.Id);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var uniqueEmail = $"login_test_{Guid.NewGuid()}@example.com";
        var user = new UserRegisterDto { Email = uniqueEmail, Password = "Password123!" };
        await _client.PostAsJsonAsync("/api/auth/register", user);

        var loginDetails = new UserLoginDto { Email = uniqueEmail, Password = "Password123!" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDetails);
        var token = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.False(string.IsNullOrWhiteSpace(token));
    }
}
