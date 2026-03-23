using Microsoft.AspNetCore.Mvc;
using InventoryAPI.DTOs;

namespace InventoryAPI.Controllers;

public partial class WarehousesController
{
    [HttpPost]
    public async Task<ActionResult> CreateNewWarehouse(
        CreateWarehouseRequest newWarehouse
    )
    {        
        var warehouse = await _warehouseService.CreateNewWarehouseAsync(newWarehouse);
            
        return CreatedAtAction(
            nameof(GetWarehouseById),
            new { id = warehouse.Id },
            warehouse
        );
    }
}