using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial GET Part
public partial class WarehouseService
{

    public async Task<List<WarehouseResponse>> GetAllAsync()
    {
        return await _dbContext.Warehouses.Where(w => w.is_active)
            .Select(w => new WarehouseResponse
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                CreatedAt = w.created_at
            })
            .ToListAsync();
    }

    public async Task<WarehouseResponse?> GetWarehouseByIdAsync(int id)
    {
        Warehouse? warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id && w.is_active);
        
        return warehouse == null ? null : ReturnWarehouseResponse(warehouse);
    }
}