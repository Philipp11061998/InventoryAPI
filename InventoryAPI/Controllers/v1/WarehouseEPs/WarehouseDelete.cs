using Microsoft.AspNetCore.Mvc;
using InventoryAPI.DTOs;

namespace InventoryAPI.Controllers;

public partial class WarehousesController
{
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteWarehouseById(
        int id    
    )
    {       
        var result = await _warehouseService.DeleteWarehouseByIdAsync(id);
            
        if(result == null) return NotFound();
        else return NoContent();
    }
}