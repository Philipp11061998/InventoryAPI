using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class WarehousesController
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {       
        var result = await _warehouseService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetWarehouseById(int id)
    {       
        var result = await _warehouseService.GetWarehouseByIdAsync(id);
            
        if(result == null) return NotFound();
        else return Ok(result);
    }
}