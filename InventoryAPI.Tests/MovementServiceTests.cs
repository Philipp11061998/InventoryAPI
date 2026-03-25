using InventoryAPI.Common;
using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Tests;

public class MovementServiceTests
{
    [Fact]
    public async Task CreateNewMovementAsync_ProductDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, product, warehouse, movementService) = await CreateTestPreparations();

        var newMovement = CreateValidMovementRequest(product, warehouse);

        newMovement.ProductId = 2;

        // Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await movementService.CreateNewMovementAsync(newMovement);
        });

        Assert.Contains("Product doesn't exist", ex.Message);

    }

    [Fact]
    public async Task CreateNewMovementAsync_WarehouseDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, product, warehouse, movementService) = await CreateTestPreparations();

        var newMovement = CreateValidMovementRequest(product, warehouse);

        newMovement.WarehouseId = 2;

        // Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await movementService.CreateNewMovementAsync(newMovement);
        });

        Assert.Contains("Warehouse doesn't exist", ex.Message);
    }

    [Fact]
    public async Task CreateNewMovementAsync_WarehouseAndProductDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, product, warehouse, movementService) = await CreateTestPreparations();

        var newMovement = CreateValidMovementRequest(product, warehouse);

        newMovement.WarehouseId = 2;
        newMovement.ProductId = 2;

        // Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await movementService.CreateNewMovementAsync(newMovement);
        });

        Assert.Contains("Product doesn't exist", ex.Message);
        Assert.Contains("Warehouse doesn't exist", ex.Message);
    }

    [Fact]
    public async Task CreateNewMovementAsync_AmountSmallerOrEqualZeo_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, product, warehouse, movementService) = await CreateTestPreparations();

        var newMovement = CreateValidMovementRequest(product, warehouse);

        newMovement.Amount = -1;

        // Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await movementService.CreateNewMovementAsync(newMovement);
        });

        Assert.Contains("Amount can't be lower or equal Zero", ex.Message);
    }

    [Fact]
    public async Task CreateNewMovementAsync_ProductIsOutOfStock_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, product, warehouse, movementService) = await CreateTestPreparations();

        var newMovement = CreateValidMovementRequest(product, warehouse);

        newMovement.MovementType = MovementType.Outbound;

        // Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await movementService.CreateNewMovementAsync(newMovement);
        });

        Assert.Contains("is only available", ex.Message);
    }

    [Fact]
    public async Task CreateNewMovementAsync_ValidInboundRequest_CreatesMovement()
    {
        // Arrange
        var (dbContext, product, warehouse, movementService) = await CreateTestPreparations();

        var newMovement = CreateValidMovementRequest(product, warehouse);

        newMovement.Amount = 21;

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
    public async Task CreateNewMovementAsync_ValidOutboundRequest_ReducesInventory()
    {
        // Arrange
        var (dbContext, product, warehouse, movementService) = await CreateTestPreparations();

        var newInboundMovement = CreateValidMovementRequest(product, warehouse);

        newInboundMovement.Amount = 10;

        await movementService.CreateNewMovementAsync(newInboundMovement);

        var newOutboundMovement = CreateValidMovementRequest(product, warehouse);

        newOutboundMovement.Amount = 4;
        newOutboundMovement.MovementType = MovementType.Outbound;


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

    [Fact]
    public async Task CreateNewMovementAsync_TwoOutboundRequests_TestRaceCondition()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new InventoryDbContext(options);
        var dbContext2 = new InventoryDbContext(options);

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
        var movementService2 = new MovementService(dbContext2);

        var newInboundMovement = CreateValidMovementRequest(product, warehouse);

        newInboundMovement.Amount = 6;

        await movementService.CreateNewMovementAsync(newInboundMovement);

        var newOutboundMovement = CreateValidMovementRequest(product, warehouse);

        newOutboundMovement.Amount = 5;
        newOutboundMovement.MovementType = MovementType.Outbound;

        var newOutboundMovement2 = CreateValidMovementRequest(product, warehouse);

        newOutboundMovement2.Amount = 5;
        newOutboundMovement2.MovementType = MovementType.Outbound;

        // Act
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await Task.WhenAll(
                movementService.CreateNewMovementAsync(newOutboundMovement),
                movementService2.CreateNewMovementAsync(newOutboundMovement2)
            );
        });

        var dbContext3 = new InventoryDbContext(options); //3. Frischer DBContext für aktuelle Datensätze
        var inventoryService = new InventoryService(dbContext3);

        var outboundMovements = dbContext3.Movements.Where(p => p.MovementType == MovementType.Outbound && 
            p.ProductId == newInboundMovement.ProductId && 
            p.WarehouseId == newInboundMovement.WarehouseId
        );

        var inventory = await inventoryService.GetFullInventoryAsync(product.Id, warehouse.Id);

        // Assert
        Assert.Contains("is only available", ex.Message);
        Assert.Single(outboundMovements);
        Assert.Equal(newInboundMovement.Amount - newOutboundMovement.Amount, inventory.First().Amount); //Hier sicher, weil beide Requests den selben Amount haben
    }

    private async Task<(InventoryDbContext dbContext, Product product, Warehouse warehouse, MovementService movementService)> CreateTestPreparations()
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

    private CreateMovementRequest CreateValidMovementRequest(Product product, Warehouse warehouse)
    {
        return new CreateMovementRequest
        {
            ProductId = product.Id,
            WarehouseId = warehouse.Id,
            Amount = 21,
            MovementType = MovementType.Inbound
        };
    }
}