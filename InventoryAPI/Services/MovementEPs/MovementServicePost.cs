using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;
using InventoryAPI.Common;

namespace InventoryAPI.Services;

//Partial POST Part
public partial class MovementService
{
    public async Task<Movement> CreateNewMovementAsync(CreateMovementRequest newMovement)
    {
        if(newMovement.Amount <= 0) throw new InvalidOperationException("Amount can't be lower or equal Zero");

        Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == newMovement.ProductId && p.is_active);
        Warehouse? warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == newMovement.WarehouseId && w.is_active);
        
        List<string> errors = new List<string>();

        if(product == null) errors.Add("Product doesn't exist");
        if(warehouse == null) errors.Add("Warehouse doesn't exist");

        if(errors.Count > 0) throw new InvalidOperationException(string.Join(". ", errors));

        var movement = new Movement
        {
            ProductId = newMovement.ProductId,
            WarehouseId = newMovement.WarehouseId,
            Amount = newMovement.Amount,
            MovementType = newMovement.MovementType,
            TransferReference = newMovement.TransferReference,
            Note = newMovement.Note
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        try 
        {
            if(newMovement.MovementType == MovementType.Outbound)
            {
                var movements = await _dbContext.Movements.Where(m => m.ProductId == newMovement.ProductId && m.WarehouseId == newMovement.WarehouseId).ToListAsync();

                int foundAmount = movements.Sum(m => m.MovementType == MovementType.Inbound ? m.Amount : -m.Amount);

                if(foundAmount < newMovement.Amount) throw new InvalidOperationException($"ProductId {newMovement.ProductId} is only available {foundAmount} times");
            }

            _dbContext.Movements.Add(movement);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        } catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return movement;
    }
}