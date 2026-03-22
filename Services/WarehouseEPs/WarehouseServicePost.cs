using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InventoryAPI.Services;

//Partial POST Part
public partial class WarehouseService
{
    public async Task<Warehouse> CreateNewWarehouseAsync(CreateWarehouseRequest newWarehouse)
    {
        var warehouse = new Warehouse
        {
            Name = newWarehouse.Name,
            Description = newWarehouse.Description
        };

        if(await _dbContext.Warehouses.AnyAsync(w => w.Name == warehouse.Name))
        {
            throw new InvalidOperationException("Warehouse with the same Name already exists.");
        }

        _dbContext.Warehouses.Add(warehouse);

        await _dbContext.SaveChangesAsync();

        return warehouse;
    }
}