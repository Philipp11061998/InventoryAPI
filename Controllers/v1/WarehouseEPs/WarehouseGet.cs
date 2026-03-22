using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class WarehousesController
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {       
        try
        {
            var result = await _warehouseService.GetAllAsync();
            return Ok(result);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetWarehouseById(int id)
    {       
        try
        {
            var result = await _warehouseService.GetWarehouseByIdAsync(id);
            
            if(result == null) return NotFound();
            else return Ok(result);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}