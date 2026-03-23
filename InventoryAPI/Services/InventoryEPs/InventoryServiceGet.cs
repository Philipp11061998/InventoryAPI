using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;
using InventoryAPI.Common;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace InventoryAPI.Services;

//Partial GET Part
public partial class InventoryService
{

    public async Task<List<Inventory>> GetFullInventoryAsync(int? productId, int? warehouseId)
    {
        var inventory = await (from movement in _dbContext.Movements
            join product in _dbContext.Products
            on movement.ProductId equals product.Id
            join warehouse in _dbContext.Warehouses
            on movement.WarehouseId equals warehouse.Id
            where product.is_active && warehouse.is_active
            && (productId == null ? true : product.Id == productId)
            && (warehouseId == null ? true : warehouse.Id == warehouseId)
            select new
            {
                ProductId = movement.ProductId,
                ProductName = product.Name,
                WarehouseId = movement.WarehouseId,
                WarehouseName = warehouse.Name,
                MovementType = movement.MovementType,
                Amount = movement.Amount
            }
            )
            .GroupBy(x => new { x.ProductId, x.WarehouseId, x.ProductName, x.WarehouseName })
            .Select(g => new Inventory
            {
                ProductId = g.Key.ProductId,
                WarehouseId = g.Key.WarehouseId,
                Amount = g.Sum(m => m.MovementType == MovementType.Inbound ? m.Amount : -m.Amount),
                ProductName = g.Key.ProductName,
                WarehouseName = g.Key.WarehouseName
            })
            .ToListAsync();
        
        return inventory;
    }
}