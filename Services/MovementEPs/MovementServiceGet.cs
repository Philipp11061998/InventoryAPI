using InventoryAPI.Data;
using InventoryAPI.Models;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial GET Part
public partial class MovementService
{

    public async Task<List<Movement>> GetAllAsync()
    {
        return await _dbContext.Movements.ToListAsync();
    }

    public async Task<Movement?> GetMovementByIdAsync(int id)
    {
        Movement? movement = await _dbContext.Movements.FirstOrDefaultAsync(p => p.Id == id);
        
        return movement;
    }
}