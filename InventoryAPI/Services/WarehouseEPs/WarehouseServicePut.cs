using InventoryAPI.Models;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using InventoryAPI.Exceptions;

namespace InventoryAPI.Services;

//Partial PUT Part
public partial class WarehouseService
{
    public async Task<WarehouseResponse?> UpdateWarehouseByIdAsync(int id, UpdateWarehouseRequest warehouseInput)
    {
        if(string.IsNullOrEmpty(warehouseInput.Name) && string.IsNullOrEmpty(warehouseInput.Description)) throw new InvalidOperationException("No changes possible. All fields are null");
        
        Warehouse? warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id);

        if(warehouse == null) return null;

        if(warehouse.IsActive == false) throw new DomainException.WarehouseInactiveException();

        if(!string.IsNullOrEmpty(warehouseInput.Name))
        {
            //Check if new Name already exists
            Warehouse? warehouseNameSearch = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Name == warehouseInput.Name && w.Id != id);
            if(warehouseNameSearch != null) throw new DomainException.WarehouseAlreadyExistsException(warehouseInput.Name);
        }

        warehouse.Name = warehouseInput.Name == null ? warehouse.Name : warehouseInput.Name;
        warehouse.Description = warehouseInput.Description == null ? warehouse.Description : warehouseInput.Description;

        await _dbContext.SaveChangesAsync();
        
        return MapToWarehouseResponse(warehouse);
    }
}