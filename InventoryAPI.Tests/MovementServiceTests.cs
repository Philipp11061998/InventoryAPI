using InventoryAPI.Common;
using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using InventoryAPI.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Tests;

public class MovementServiceTests
{
    [Fact]
    public async Task CreateNewMovementAsyncThrowErrorIfProductIsOutOfStock()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new InventoryDbContext(options);

        await dbContext.Database.EnsureCreatedAsync();

        var product = new Product
        {
            Sku = "test-product",
            Name = "Test Produkt",
            Description = null,
            is_active = true
        };

        dbContext.Products.Add(product);

        var warehouse = new Warehouse
        {
            Name = "Test Warehouse",
            Description = null,
            is_active = true
        };

        dbContext.Warehouses.Add(warehouse);

        await dbContext.SaveChangesAsync();

        var movementService = new MovementService(dbContext);

        var newMovement = new CreateMovementRequest
        {
            ProductId = product.Id,
            WarehouseId = warehouse.Id,
            Amount = 1,
            MovementType = MovementType.Outbound
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await movementService.CreateNewMovementAsync(newMovement);
        });
    }
}