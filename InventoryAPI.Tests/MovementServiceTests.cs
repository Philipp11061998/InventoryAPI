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
        // Arrange
        var (dbContext, product, warehouse, movementService) = await CreateTestPreparations();

        var newMovement = new CreateMovementRequest
        {
            ProductId = product.Id,
            WarehouseId = warehouse.Id,
            Amount = 1,
            MovementType = MovementType.Outbound
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await movementService.CreateNewMovementAsync(newMovement);
        });
    }

    [Fact]
    public async Task CreateNewMovementAsync_ShouldCreateInboundMovement()
    {
        // Arrange
        var (dbContext, product, warehouse, movementService) = await CreateTestPreparations();

        var newMovement = new CreateMovementRequest
        {
            ProductId = product.Id,
            WarehouseId = warehouse.Id,
            Amount = 21,
            MovementType = MovementType.Inbound
        };

        // Act
        var movementResult = await movementService.CreateNewMovementAsync(newMovement);
        var movementSearch = dbContext.Movements.FirstOrDefault(m => m.Id == movementResult.Id);

        // Assert
        Assert.NotNull(movementResult);
        Assert.NotNull(movementSearch);
        Assert.Equal(newMovement.Amount, movementSearch.Amount);
        Assert.Equal(newMovement.ProductId, movementSearch.ProductId);
        Assert.Equal(MovementType.Inbound, movementSearch.MovementType);
        Assert.Equal(newMovement.WarehouseId, movementSearch.WarehouseId);
    }

    [Fact]
    public async Task CreateNewMovementAsync_OutboundShouldReduceInventory()
    {
        // Arrange
        var (dbContext, product, warehouse, movementService) = await CreateTestPreparations();

        var newInboundMovement = new CreateMovementRequest
        {
            ProductId = product.Id,
            WarehouseId = warehouse.Id,
            Amount = 10,
            MovementType = MovementType.Inbound
        };

        await movementService.CreateNewMovementAsync(newInboundMovement);

        var newOutboundMovement = new CreateMovementRequest
        {
            ProductId = product.Id,
            WarehouseId = warehouse.Id,
            Amount = 4,
            MovementType = MovementType.Outbound
        };

        var inventoryService = new InventoryService(dbContext);

        // Act
        var outboundMovementResult = await movementService.CreateNewMovementAsync(newOutboundMovement);
        var outboundMovementSearch = dbContext.Movements.FirstOrDefault(m => m.Id == outboundMovementResult.Id);
        var inventorySearch = await inventoryService.GetFullInventoryAsync(product.Id, warehouse.Id);

        // Assert
        Assert.NotNull(outboundMovementSearch);
        Assert.Equal(newOutboundMovement.Amount, outboundMovementSearch.Amount);
        Assert.Equal(newOutboundMovement.ProductId, outboundMovementSearch.ProductId);
        Assert.Equal(MovementType.Outbound, outboundMovementSearch.MovementType);
        Assert.Equal(newOutboundMovement.WarehouseId, outboundMovementSearch.WarehouseId);

        Assert.Single(inventorySearch);
        Assert.Equal(newInboundMovement.Amount - newOutboundMovement.Amount, inventorySearch.First().Amount);
    }

    public async Task<(InventoryDbContext dbContext, Product product, Warehouse warehouse, MovementService movementService)> CreateTestPreparations()
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

        return (dbContext, product, warehouse, movementService);
    }
}