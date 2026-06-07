using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using InventoryAPI.DTOs;
using InventoryAPI.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace InventoryAPI.Tests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        }).CreateClient();
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task RegisterAsync_ValidRequest_Returns201()
    {
        // Arrange 
        var registerRequest = new Register
        {
            Username = "NewUser",
            Password = "NewUser123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);
        var content = await response.Content.ReadAsStringAsync();

        var userJson = JsonDocument.Parse(content);
        var userName = userJson.RootElement.GetProperty("username").GetString();
        var userRole = userJson.RootElement.GetProperty("role").GetString();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEmpty(content);
        Assert.Equal(registerRequest.Username, userName);
        Assert.Equal("User", userRole);
        Assert.False(userJson.RootElement.TryGetProperty("PasswordHash", out var passwordHash));
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task RegisterAsync_UserAlreadyExists_Returns401()
    {
        // Arrange
        var registerRequest = new Register
        {
            Username = "User",
            Password = "User123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotEmpty(content);
        Assert.Contains($"User with Username '{registerRequest.Username}' already exists", content);
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task RegisterAsync_EmptyBody_Returns400()
    {
        // Act (kein Arrange nötig)
        var response = await _client.PostAsJsonAsync("/api/Auth/register", new {});
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(content);
    }

    [Trait("Category", "Integration")]      
    [Fact]
    public async Task LoginAsync_ValidCredentials_Returns200WithToken()
    {
        // Arrange
        var loginRequest = new Login
        {
            Username = "User",
            Password = "User123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(content);
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task LoginAsync_WrongPassword_Returns401()
    {
        // Arrange
        var loginRequest = new Login
        {
            Username = "User",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEmpty(content);
        Assert.Contains("Passwort falsch!", content);
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task LoginAsync_NonExistingUser_Returns401()
    {
        // Arrange
        var loginRequest = new Login
        {
            Username = "NonExisting",
            Password = "NonExisting123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEmpty(content);
        Assert.Contains(DomainException.UserNotFoundException.ERROR_MESSAGE, content);
    }


    [Trait("Category", "Integration")]
    [Fact]
    public async Task LoginAsync_EmptyBody_Returns400()
    {
        // Act (kein Arrange nötig)
        var response = await _client.PostAsJsonAsync("/api/Auth/login", new {});
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(content);
    }
}