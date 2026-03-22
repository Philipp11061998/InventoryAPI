using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;
using InventoryAPI.Common;
using System.Linq;

namespace InventoryAPI.Services;

//Partial POST Part
public partial class MovementService
{
    public async Task<InventoryMovement> CreateNewMovementAsync(CreateMovementRequest newMovement)
    {
        Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == newMovement.ProductId && p.is_active);
        Warehouse? warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == newMovement.WarehouseId && w.is_active);
        
        List<string> errors = new List<string>();

        if(product == null) errors.Add("Product doesn't exist");
        if(warehouse == null) errors.Add("Warehouse doesn't exist");

        if(errors.Count > 0) throw new InvalidOperationException(string.Join(". ", errors));

        if(newMovement.MovementType == MovementType.Outbound)
        {
            var movements = await _dbContext.Movements.Where(m => m.ProductId == newMovement.ProductId && m.WarehouseId == newMovement.WarehouseId).ToListAsync();

            int foundAmount = movements.Sum(m => m.MovementType == MovementType.Inbound ? m.Amount : -m.Amount);

            if(foundAmount < newMovement.Amount) throw new InvalidOperationException($"ProductId {newMovement.ProductId} is only available {foundAmount} times");
        }
        

        var movement = new InventoryMovement
        {
            ProductId = newMovement.ProductId,
            WarehouseId = newMovement.WarehouseId,
            Amount = newMovement.Amount,
            MovementType = newMovement.MovementType,
            TransferReference = newMovement.TransferReference,
            Note = newMovement.Note
        };

        _dbContext.Movements.Add(movement);

        await _dbContext.SaveChangesAsync();

        return movement;
    }
}