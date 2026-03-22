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
        try
        {
            var result = await _warehouseService.DeleteWarehouseByIdAsync(id);
            
            if(result == null) return NotFound();
            else return NoContent();
        } 
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}