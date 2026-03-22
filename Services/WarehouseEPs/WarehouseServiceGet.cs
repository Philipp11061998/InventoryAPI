using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial GET Part
public partial class WarehouseService
{

    public async Task<List<Warehouse>> GetAllAsync()
    {
        return await _dbContext.Warehouses.Where(w => w.is_active)
            .ToListAsync();
    }

    public async Task<Warehouse?> GetWarehouseByIdAsync(int id)
    {
        Warehouse? warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id && w.is_active);
        
        return warehouse;
    }
}