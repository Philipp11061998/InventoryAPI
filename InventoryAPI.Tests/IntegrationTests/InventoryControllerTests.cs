using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using InventoryAPI.DTOs;
using InventoryAPI.Exceptions;
using InventoryAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Query;

namespace InventoryAPI.Tests;

public class InventoryControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private record InventoryItem(int ProductId, int WarehouseId);

    public InventoryControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        }).CreateClient();
    }

    public async Task InitializeAsync()
    {
        await AddUserToken();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Trait("Category", "Integration")]
    [Fact]
    public async Task GetFullInventoryAsync_ValidRequestWithoutFilters_Returns200AndNonEmptyList()
    {
        // Act (kein Arrange nötig, da Token im Constructor added wird)
        var response = await _client.GetAsync("/api/Inventory");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(content);

    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task GetFullInventoryAsync_ValidRequestWithProductFilter_Returns200AndNonEmptyList()
    {
        // Act (kein Arrange nötig, da Token im Constructor added wird)
        var response = await _client.GetAsync("/api/Inventory?productId=1");
        var content = await response.Content.ReadAsStringAsync();
        var jsonContent = JsonSerializer.Deserialize<List<InventoryItem>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(content);
        Assert.NotNull(jsonContent);
        Assert.NotEmpty(jsonContent);
        Assert.All(jsonContent, inv => Assert.Equal(1, inv.ProductId));
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task GetFullInventoryAsync_ValidRequestWithWarehouseFilter_Returns200AndNonEmptyList()
    {
        // Act (kein Arrange nötig, da Token im Constructor added wird)
        var response = await _client.GetAsync("/api/Inventory?warehouseId=1");
        var content = await response.Content.ReadAsStringAsync();
        var jsonContent = JsonSerializer.Deserialize<List<InventoryItem>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(content);
        Assert.NotNull(jsonContent);
        Assert.NotEmpty(jsonContent);
        Assert.All(jsonContent, inv => Assert.Equal(1, inv.WarehouseId));
    }

    private async Task AddUserToken()
    {
        var loginRequest = new Login
        {
            Username = "User",
            Password = "User123!"
        };

        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + content);
    }
}