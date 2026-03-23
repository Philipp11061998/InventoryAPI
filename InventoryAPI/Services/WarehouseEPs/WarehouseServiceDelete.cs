using InventoryAPI.Models;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial PUT Part
public partial class WarehouseService
{
    public async Task<WarehouseResponse?> DeleteWarehouseByIdAsync(int id)
    {        
        Warehouse? warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id);

        if(warehouse == null) return null;

        if(warehouse.is_active == false) throw new InvalidOperationException("Warehouse already inactive");

        warehouse.is_active = false;

        await _dbContext.SaveChangesAsync();
        
        return MapToWarehouseResponse(warehouse);
    }
}