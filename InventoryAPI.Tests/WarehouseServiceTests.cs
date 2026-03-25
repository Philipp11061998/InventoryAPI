using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using InventoryAPI.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Tests;

public class WarehouseServiceTests
{
    [Fact]
    public async Task DeleteWarehouseAsync_WarehouseAlreadyInactive_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();

        // Act & Assert
        await warehouseService.DeleteWarehouseByIdAsync(warehouse.Id); //Erstes Löschen um auf inaktiv zu setzen (keine eigene Datenbankaktion, sondern nutzen der bestehenden Infrastruktur)

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await warehouseService.DeleteWarehouseByIdAsync(warehouse.Id);
        }); //Zweite Löschung wirft dann die Exception

        Assert.Contains("Warehouse already inactive", ex.Message);

    }

    [Fact]
    public async Task DeleteWarehouseAsync_WarehouseNotFound_ReturnNull()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations(); //Nur Nutzen um Service und DB aufzubauen

        // Act & Assert
        var result = await warehouseService.DeleteWarehouseByIdAsync(2); //Nicht vorhandene Id

        Assert.Null(result);

    }

    [Fact]
    public async Task DeleteWarehouseAsync_ActiveWarehouse_ReturnsResponseAndSetsIsActiveFalse()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations(); //Nur Nutzen um Service und DB aufzubauen

        // Act & Assert
        var result = await warehouseService.DeleteWarehouseByIdAsync(warehouse.Id); //Nicht vorhandene Id
        var warehouseSearch = await dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == warehouse.Id);

        Assert.NotNull(result);
        Assert.False(warehouseSearch?.is_active);
        Assert.Equal(warehouse.Id, result.Id);
        Assert.Equal(warehouse.Name, result.Name);
        Assert.Equal(warehouse.Description, result.Description);
        Assert.Equal(warehouse.created_at, result.CreatedAt);

    }

    [Fact]
    public async Task GetAllAsync_GetOnlyActiveWarehouses_ReturnsOnlyActiveWarehouses()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations(); //Nur Nutzen um Service und DB aufzubauen
        var newWarehouse = new CreateWarehouseRequest
        {
            Name = "Test inaktiv",
        };

        var createdWarehouse = await warehouseService.CreateNewWarehouseAsync(newWarehouse);
        await warehouseService.DeleteWarehouseByIdAsync(createdWarehouse.Id);

        await dbContext.SaveChangesAsync();


        // Act & Assert
        var result = await warehouseService.GetAllAsync(); //Nicht vorhandene Id

        Assert.Single(result);
        Assert.Equal(warehouse.Id, result.First().Id);
        Assert.Equal(warehouse.Name, result.First().Name);
        Assert.Equal(warehouse.Description, result.First().Description);
        Assert.DoesNotContain(result, w => w.Id == createdWarehouse.Id);
    }

    [Fact]
    public async Task GetWarehouseByIdAsync_ActiveWarehouse_ReturnsWarehouseResponse()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();

        // Act
        var warehouseByEndpoint = await warehouseService.GetWarehouseByIdAsync(warehouse.Id);

        // Assert
        Assert.NotNull(warehouseByEndpoint);
        Assert.Equal(warehouse.Id, warehouseByEndpoint.Id);
        Assert.Equal(warehouse.Name, warehouseByEndpoint.Name);
    }

    [Fact]
    public async Task GetWarehouseByIdAsync_WarehouseDoesNotExist_ReturnsNull()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();

        // Act
        WarehouseResponse? warehouseSearch = await warehouseService.GetWarehouseByIdAsync(2); //Produkt mit Id 2 kann nicht existieren

        //Assert
        Assert.Null(warehouseSearch);
    }

    [Fact]
    public async Task GetWarehouseByIdAsync_InactiveWarehouse_ReturnsNull()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();
        await warehouseService.DeleteWarehouseByIdAsync(warehouse.Id);

        // Act
        WarehouseResponse? warehouseSearch = await warehouseService.GetWarehouseByIdAsync(warehouse.Id);

        //Assert
        Assert.Null(warehouseSearch);
    }

    [Fact]
    public async Task CreateNewWarehouseAsync_NameAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();

        CreateWarehouseRequest newWarehouseRequest = new CreateWarehouseRequest
        {
            Name = warehouse.Name,
        };

        // Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await warehouseService.CreateNewWarehouseAsync(newWarehouseRequest);
        });

        Assert.Contains("Warehouse with the same Name already exists", ex.Message);

    }

    [Fact]
    public async Task CreateNewWarehouseAsync_ValidWarehouse_ReturnsCreatedWarehouse()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();

        CreateWarehouseRequest newWarehouseRequest = new CreateWarehouseRequest
        {
            Name = "Test Warehouse 2",
        };

        //Act
        var warehouseInsert = await warehouseService.CreateNewWarehouseAsync(newWarehouseRequest);
        var warehouseDbCheck = dbContext.Warehouses.FirstOrDefault(w => w.Id == warehouseInsert.Id);

        //Assert
        Assert.NotNull(warehouseInsert);
        Assert.NotNull(warehouseDbCheck);
        Assert.Equal(warehouseInsert.Id, warehouseDbCheck.Id);
        Assert.Equal(newWarehouseRequest.Name, warehouseInsert.Name);
        Assert.Equal(newWarehouseRequest.Name, warehouseDbCheck.Name);
    }

    [Fact]
    public async Task UpdateWarehouseByIdAsync_ValidUpdate_ReturnsUpdatedWarehouse()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();
        
        var updateWarehouseRequest = new UpdateWarehouseRequest
        {
            Name = "Test Update",
        };

        // Act
        var result = await warehouseService.UpdateWarehouseByIdAsync(warehouse.Id, updateWarehouseRequest);
        var databaseWarehouse = dbContext.Warehouses.FirstOrDefault(w => w.Id == warehouse.Id);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(warehouse.Id, result.Id);
        Assert.Equal(updateWarehouseRequest.Name, result.Name);
        Assert.NotNull(databaseWarehouse);
        Assert.Equal(warehouse.Id, databaseWarehouse.Id);
        Assert.Equal(updateWarehouseRequest.Name, databaseWarehouse.Name);
    }

    [Fact]
    public async Task UpdateWarehouseByIdAsync_UpdateWithInvalidParameters_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();
        
        var updateWarehouseRequest = new UpdateWarehouseRequest
        {
            Name = null,
            Description = null
        };

        //Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
           await warehouseService.UpdateWarehouseByIdAsync(warehouse.Id, updateWarehouseRequest);
        });

        Assert.Contains("No changes possible", ex.Message);
    }

    [Fact]
    public async Task UpdateWarehouseByIdAsync_UpdateNotExistingWarehouse_ReturnsNull()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();
        
        var updateWarehouseRequest = new UpdateWarehouseRequest
        {
            Name = "Test Update",
        };

        //Act
        var result = await warehouseService.UpdateWarehouseByIdAsync(2, updateWarehouseRequest);

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateWarehouseByIdAsync_UpdateInactiveWarehouse_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();
        await warehouseService.DeleteWarehouseByIdAsync(warehouse.Id);
        
        var updateWarehouseRequest = new UpdateWarehouseRequest
        {
            Name = "Test Update",
        };

        //Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await warehouseService.UpdateWarehouseByIdAsync(warehouse.Id, updateWarehouseRequest);
        });

        Assert.Contains("Warehouse inactive", ex.Message);
    }

    [Fact]
    public async Task UpdateWarehouseByIdAsync_UpdateWarehouseToExistingName_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, warehouse, warehouseService) = await CreateTestPreparations();

        CreateWarehouseRequest warehouseRequest = new CreateWarehouseRequest
        {
            Name = "Test Update",
        };

        await warehouseService.CreateNewWarehouseAsync(warehouseRequest);
        
        var updateWarehouseRequest = new UpdateWarehouseRequest
        {
            Name = "Test Update",
        };

        //Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await warehouseService.UpdateWarehouseByIdAsync(warehouse.Id, updateWarehouseRequest);
        });

        Assert.Contains("Name already exists", ex.Message);
    }
 
    private async Task<(InventoryDbContext dbContext, Warehouse warehouse, WarehouseService warehouseService)> CreateTestPreparations()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new InventoryDbContext(options);

        await dbContext.Database.EnsureCreatedAsync();

        var warehouse = new Warehouse
        {
            Name = "Test Warehouse",
            Description = null,
            is_active = true
        };

        dbContext.Warehouses.Add(warehouse);

        await dbContext.SaveChangesAsync();

        var warehouseService = new WarehouseService(dbContext);

        return (dbContext, warehouse, warehouseService);
    }
}