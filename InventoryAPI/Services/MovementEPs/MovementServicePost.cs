using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;
using InventoryAPI.Common;
using InventoryAPI.Exceptions;

namespace InventoryAPI.Services;

//Partial POST Part
public partial class MovementService
{
    public async Task<Movement> CreateNewMovementAsync(CreateMovementRequest newMovement)
    {
        Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == newMovement.ProductId && p.IsActive);
        Warehouse? warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == newMovement.WarehouseId && w.IsActive);
        
        List<ValidationError> errors = new List<ValidationError>();

        if(product == null) errors.Add(new ValidationError(
            DomainException.ProductNotFoundException.ERROR_CODE, 
            DomainException.ProductNotFoundException.ERROR_MESSAGE));

        if(warehouse == null) errors.Add(new ValidationError(
            DomainException.WarehouseNotFoundException.ERROR_CODE,
            DomainException.WarehouseNotFoundException.ERROR_MESSAGE
        ));

        if(errors.Count > 0) throw new ValidationException(errors);

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

                if(foundAmount < newMovement.Amount) throw new DomainException.InsufficientStockException(newMovement.ProductId, newMovement.WarehouseId);
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