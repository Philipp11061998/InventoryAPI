using InventoryAPI.Models;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial PUT Part
public partial class WarehouseService
{
    public async Task<WarehouseResponse?> UpdateWarehouseByIdAsync(int id, UpdateWarehouseRequest warehouseInput)
    {
        if(string.IsNullOrEmpty(warehouseInput.Name) && string.IsNullOrEmpty(warehouseInput.Description)) throw new InvalidOperationException("No changes possible. All fields are null");
        
        Warehouse? warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id);

        if(warehouse == null) return null;

        if(warehouse.is_active == false) throw new InvalidOperationException("Warehouse inactive. No actions possible");

        if(string.IsNullOrEmpty(warehouse.Name))
        {
            //Check if new Sku already exists
            Warehouse? warehouseNameSearch = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Name == warehouseInput.Name && w.Id != id);
            if(warehouseNameSearch != null) throw new InvalidOperationException("Name already exists. No change possible");
        }

        warehouse.Name = warehouseInput.Name == null ? warehouse.Name : warehouseInput.Name;
        warehouse.Description = warehouse.Description == null ? warehouse.Description : warehouse.Description;

        await _dbContext.SaveChangesAsync();
        
        return MapToWarehouseResponse(warehouse);
    }
}