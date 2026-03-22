using InventoryAPI.Data;
using InventoryAPI.Models;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial GET Part
public partial class MovementService
{

    public async Task<List<InventoryMovement>> GetAllAsync()
    {
        return await _dbContext.Movements.ToListAsync();
    }

    public async Task<InventoryMovement?> GetMovementByIdAsync(int id)
    {
        InventoryMovement? movement = await _dbContext.Movements.FirstOrDefaultAsync(p => p.Id == id);
        
        return movement;
    }
}