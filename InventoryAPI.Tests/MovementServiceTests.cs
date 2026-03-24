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

    [Fact]
    public async Task CreateNewMovementAsync_ShouldCreateInboundMovement()
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

        var productSearch = dbContext.Products.Where(p => p.Id == product.Id);
        var warehouseSearch = dbContext.Warehouses.Where(wh => wh.Id == warehouse.Id);

        //Check ob beide Einträge valide waren
        Assert.True(productSearch.Count() > 0 && warehouseSearch.Count() > 0);

        var movementService = new MovementService(dbContext);

        var newMovement = new CreateMovementRequest
        {
            ProductId = product.Id,
            WarehouseId = warehouse.Id,
            Amount = 21,
            MovementType = MovementType.Inbound
        };

        var movementResult = await movementService.CreateNewMovementAsync(newMovement);

        //Check ob Movement korrekt erstellt wurde
        Assert.True(movementResult != null);

        var movementSearch = dbContext.Movements.FirstOrDefault(m => m.Id == movementResult.Id);

        //Check ob vorhanden und richtige Werte gesetzt wurden
        Assert.True(
            movementSearch != null &&
            movementSearch.Amount == newMovement.Amount && 
            movementSearch.ProductId == newMovement.ProductId &&
            movementSearch.MovementType == MovementType.Inbound &&
            movementSearch.WarehouseId == newMovement.WarehouseId
        );
    }

    [Fact]
    public async Task CreateNewMovementAsync_OutboundShouldReduceInventory()
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

        var productSearch = dbContext.Products.Where(p => p.Id == product.Id);
        var warehouseSearch = dbContext.Warehouses.Where(wh => wh.Id == warehouse.Id);

        //Check ob beide Einträge valide waren
        Assert.True(productSearch.Count() > 0 && warehouseSearch.Count() > 0);

        var movementService = new MovementService(dbContext);

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

        var outboundMovementResult = await movementService.CreateNewMovementAsync(newOutboundMovement);

        var outboundMovementSearch = dbContext.Movements.FirstOrDefault(m => m.Id == outboundMovementResult.Id);
        
        //Check ob alles richtig gesetzt wurde
        Assert.True(
            outboundMovementSearch != null &&
            outboundMovementSearch.Amount == newOutboundMovement.Amount && 
            outboundMovementSearch.ProductId == newOutboundMovement.ProductId &&
            outboundMovementSearch.MovementType == MovementType.Outbound &&
            outboundMovementSearch.WarehouseId == newOutboundMovement.WarehouseId
        );

        //Check ob neuer Bestand korrekt ist
        var inventoryService = new InventoryService(dbContext);

        var inventorySearch = await inventoryService.GetFullInventoryAsync(product.Id, warehouse.Id);

        Assert.True(
            inventorySearch.Count() == 1 && 
            inventorySearch.ElementAt(0).Amount == newInboundMovement.Amount - newOutboundMovement.Amount
        );
    }
}