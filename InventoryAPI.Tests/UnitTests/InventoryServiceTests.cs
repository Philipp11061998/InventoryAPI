using InventoryAPI.Data;
using InventoryAPI.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using InventoryAPI.Common;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Builder;

namespace InventoryAPI.Tests.UnitTests;

public class InventoryServiceTests
{
    [Fact]
    public async Task GetFullInventoryAsync_InboundAndOutboundMovements_ReturnsCalculatedAmount()
    {
        // Arrange
        var (dbContext, warehouse, product, inventoryService, movement1, movement2) = await CreateTestPreparations();

        // Act
        var result = await inventoryService.GetFullInventoryAsync(product.Id, warehouse.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal(movement1.Amount - movement2.Amount, result.First().Amount);
    }

    private async Task<(InventoryDbContext dbContext, Warehouse warehouse, Product product, InventoryService inventoryService, Movement movement1, Movement movement2)> CreateTestPreparations()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new InventoryDbContext(options);
        InventoryService inventoryService = new InventoryService(dbContext);

        await dbContext.Database.EnsureCreatedAsync();

        var warehouse = new Warehouse
        {
            Name = "Test Warehouse",
            Description = null,
            IsActive = true
        };

        var product = new Product
        {
            Name = "Test Produkt",
            Sku = "test-produkt",
            IsActive = true
        };

        dbContext.Warehouses.Add(warehouse);
        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync();

        var movement1 = new Movement
        {
            WarehouseId = warehouse.Id,
            ProductId = product.Id,
            Amount = 5,
            MovementType = MovementType.Inbound
        };

        var movement2 = new Movement
        {
            WarehouseId = warehouse.Id,
            ProductId = product.Id,
            Amount = 2,
            MovementType = MovementType.Outbound
        };

        dbContext.Movements.Add(movement1);
        dbContext.Movements.Add(movement2);

        await dbContext.SaveChangesAsync();

        return (dbContext, warehouse, product, inventoryService, movement1, movement2);
    }
}